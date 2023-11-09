using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using HttpMultipartParser;
using Newtonsoft.Json;
using NLog;
using WinAnprSqe.Helper;
using WinAnprSqe.Models;
using WinAnprSqe.Models.EqsModels;

namespace WinAnprSqe.Server
{
    public class ApiServer
    {
        private readonly HttpListener _httpListener;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static EventNotificationAlert _notificationAlert;
        private MainForm _mainForm;
        private bool _isStd;

        public ApiServer(string prefix, MainForm mainForm, bool isStd)
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(prefix);
            _mainForm = mainForm;
            _isStd = isStd;
        }

        public void Start()
        {
            _httpListener.Start();
            Logger.Info("API server started...");
            Task.Run(ListenForRequests);
        }

        public void Stop()
        {
            _httpListener.Stop();
            Logger.Info("API server stopped...");
        }

        private async Task ListenForRequests()
        {
            while (_httpListener.IsListening)
            {
                try
                {
                    var context = await _httpListener.GetContextAsync();
                    await HandleRequest(context);
                }
                catch (HttpListenerException ex)
                {
                    Logger.Error(ex);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
        }

        private async Task HandleRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/inline")
            {
                var isTest = Convert.ToBoolean(ConfigurationManager.AppSettings["IsTest"]);

                if (isTest)
                {
                    var newCar1 = new CarInlineViewModel
                    {
                        ServiceName = "Test service",
                        LicensePlate = "1KG123AAA",
                        Date = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                        Talon = "A1"
                    };

                    if (_mainForm.InvokeRequired)
                    {
                        _mainForm.Invoke(new Action(() => AddCarToQueue(newCar1)));
                    }
                    else
                    {
                        AddCarToQueue(newCar1);
                    }

                    ResponseClose(response, "OK");
                    return;
                }

                if (_notificationAlert is null)
                {
                    ResponseClose(response, "Номеров нету, на очередь!");
                    return;
                }

                var serviceId = _isStd 
                    ? ConfigurationManager.AppSettings["ServiceIdStd"] 
                    : ConfigurationManager.AppSettings["ServiceIdTec"];
                
                var serverUrl = ConfigurationManager.AppSettings["ServerUrl"];
                var requestUri = new Uri(serverUrl);

                using (var httpClient = new HttpClient())
                {
                    // Define the request content as JSON
                    var content = new StringContent(
                        "{\"jsonrpc\": \"2.0\", \"method\": \"Поставить в очередь\", \"params\": {\"service_id\": \"" +
                        serviceId +
                        "\", \"text_data\": \"" + _notificationAlert.LicensePlate + "\"}}",
                        Encoding.UTF8,
                        "application/json"
                    );

                    var responseFromSeo = await httpClient.PostAsync(requestUri, content);

                    if (responseFromSeo.IsSuccessStatusCode)
                    {
                        var responseContent = await responseFromSeo.Content.ReadAsStringAsync();

                        var desResponse = JsonConvert.DeserializeObject<ResponseEqs>(responseContent);
                        var customer = desResponse.Result.Customer;

                        var newCar = new CarInlineViewModel
                        {
                            ServiceName = customer.ToService.Name,
                            LicensePlate = customer.InputData,
                            Date = customer.StandTime,
                            Talon = $"{customer.Prefix}{customer.Number}"
                        };

                        if (_mainForm.InvokeRequired)
                        {
                            _mainForm.Invoke(new Action(() => AddCarToQueue(newCar)));
                        }
                        else
                        {
                            AddCarToQueue(newCar);
                        }

                        _notificationAlert = null;

                        // Print talon for driver in queue
                        PrinterHelper.NewCar = newCar;
                        PrinterHelper.Print();
                    }
                }

                ResponseClose(response, "OK");
                return;
            }

            if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/anpr/event")
            {
                Logger.Info("Запрос от камеры пришел!");

                if (!MultipartRequestIsValid(request, out var xmlData))
                {
                    ResponseClose(response, "Нету xml документа!");
                    return;
                }

                try
                {
                    var xdoc = XDocument.Parse(xmlData);

                    var eventType = (string)xdoc.Root.Element("{http://www.hikvision.com/ver20/XMLSchema}eventType");
                    var dateTime = (string)xdoc.Root.Element("{http://www.hikvision.com/ver20/XMLSchema}dateTime");
                    var licensePlate = (string)xdoc.Root.Element("{http://www.hikvision.com/ver20/XMLSchema}ANPR")
                        ?.Element("{http://www.hikvision.com/ver20/XMLSchema}licensePlate");

                    Logger.Info($"Данные из xml: {eventType}, {dateTime}, {licensePlate}");

                    if (eventType is "ANPR")
                    {
                        _notificationAlert = licensePlate is "unknown"
                            ? null
                            : new EventNotificationAlert
                            {
                                LicensePlate = licensePlate,
                                DateTime = dateTime,
                                EventType = eventType
                            };
                    }
                    else
                    {
                        _notificationAlert = null;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Error parsing XML: " + ex.Message);
                }

                ResponseClose(response, "OK");
            }
        }

        #region Private Methods

        private bool MultipartRequestIsValid(HttpListenerRequest request, out string xmlData)
        {
            if (request.HasEntityBody)
            {
                var parser = MultipartFormDataParser.Parse(request.InputStream);

                var formFile = parser.Files.First();

                using (var reader = new StreamReader(formFile.Data))
                {
                    xmlData = reader.ReadToEnd();
                    return true;
                }
            }

            xmlData = null;
            return false;
        }

        private void ResponseClose(HttpListenerResponse response, string text)
        {
            var buffer = Encoding.UTF8.GetBytes(text);
            response.ContentType = "text/plain";
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.Close();
        }

        private void AddCarToQueue(CarInlineViewModel newCar)
        {
            if (_isStd)
            {
                _mainForm?.CarsStandart.Insert(0, newCar);

                if (_mainForm?.CarsStandart != null && _mainForm?.CarsStandart.Count > 50)
                    _mainForm.CarsStandart.RemoveAt(_mainForm.CarsStandart.Count - 1);
            }
            else
            {
                _mainForm?.CarsTec.Insert(0, newCar);

                if (_mainForm?.CarsTec != null && _mainForm?.CarsTec.Count > 50)
                    _mainForm.CarsTec.RemoveAt(_mainForm.CarsTec.Count - 1);
            }
        }

        #endregion
    }
}