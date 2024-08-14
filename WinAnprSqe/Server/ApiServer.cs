using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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

        private EventNotificationAlert _notificationAlert;
        private MainForm _mainForm;

        public ApiServer(string prefix, MainForm mainForm)
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(prefix);
            _mainForm = mainForm;
            _notificationAlert = null;
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

        public async Task AddToQueueAsync(string buttonName, bool isStandart)
        {
            Logger.Info($"Кнопка нажата: {buttonName}!");
            var isTest = Convert.ToBoolean(ConfigurationManager.AppSettings["IsTest"]);

            if (isTest)
            {
                var testCar = new CarInlineViewModel
                {
                    ServiceName = "Test service",
                    LicensePlate = "1KG123AAA",
                    Date = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                    Talon = "A1"
                };

                if (_mainForm.InvokeRequired)
                {
                    _mainForm.Invoke(new Action(() => AddCarToQueue(testCar, isStandart)));
                }
                else
                {
                    AddCarToQueue(testCar, isStandart);
                }

                return;
            }

            if (_notificationAlert is null)
            {
                MessageBox.Show("Номеров нету, на очередь!");
                return;
            }

            if (_notificationAlert.LicensePlate is "unknown" && _notificationAlert.DateTime == null)
            {
                var unknownCar = new CarInlineViewModel
                {
                    ServiceName = "-",
                    LicensePlate = "Авто-номер не распознан",
                    Date = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                    Talon = "-"
                };

                if (_mainForm.InvokeRequired)
                {
                    _mainForm.Invoke(new Action(() => AddCarToQueue(unknownCar, isStandart)));
                }
                else
                {
                    AddCarToQueue(unknownCar, isStandart);
                }

                MessageBox.Show("Авто-номер не распознан");
                return;
            }

            var serviceId = isStandart
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
                        _mainForm.Invoke(new Action(() => AddCarToQueue(newCar, isStandart)));
                    }
                    else
                    {
                        AddCarToQueue(newCar, isStandart);
                    }

                    _notificationAlert = null;

                    // Print talon for driver in queue
                    PrinterHelper.NewCar = newCar;
                    PrinterHelper.Print();
                }
            }
        }

        private async Task HandleRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/anpr/event")
            {
                try
                {
                    if (!MultipartRequestIsValid(request, out var xmlData))
                    {
                        ResponseClose(response, "Нету xml документа!");
                        return;
                    }

                    var xdoc = XDocument.Parse(xmlData);

                    var eventType = (string)xdoc.Root.Element("{http://www.hikvision.com/ver20/XMLSchema}eventType");
                    var dateTime = (string)xdoc.Root.Element("{http://www.hikvision.com/ver20/XMLSchema}dateTime");
                    var licensePlate = (string)xdoc.Root.Element("{http://www.hikvision.com/ver20/XMLSchema}ANPR")
                        ?.Element("{http://www.hikvision.com/ver20/XMLSchema}licensePlate");

                    if (eventType is "ANPR")
                    {
                        Logger.Info($"Запрос от камеры пришел IP: {_httpListener.Prefixes.First()}!");
                        Logger.Info($"Данные из xml: {eventType}, {dateTime}, {licensePlate}");

                        _notificationAlert = licensePlate is "unknown"
                            ? new EventNotificationAlert
                            {
                                LicensePlate = licensePlate,
                                DateTime = null,
                                EventType = eventType
                            }
                            : new EventNotificationAlert
                            {
                                LicensePlate = licensePlate,
                                DateTime = dateTime,
                                EventType = eventType
                            };
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
            if (request != null && request.HasEntityBody)
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

        private void AddCarToQueue(CarInlineViewModel newCar, bool isStandart)
        {
            if (isStandart)
            {
                _mainForm?.CarsStandart.Insert(0, newCar);

                if (_mainForm?.CarsStandart != null && _mainForm?.CarsStandart.Count > 50)
                    _mainForm.CarsStandart.RemoveAt(_mainForm.CarsStandart.Count - 1);

                Logger.Info($"Машина добавлена в Standart");
            }
            else
            {
                _mainForm?.CarsTec.Insert(0, newCar);

                if (_mainForm?.CarsTec != null && _mainForm?.CarsTec.Count > 50)
                    _mainForm.CarsTec.RemoveAt(_mainForm.CarsTec.Count - 1);

                Logger.Info($"Машина добавлена в VIP");
            }
        }

        #endregion
    }
}