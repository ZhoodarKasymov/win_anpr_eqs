using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;
using WinAnprSqe.Helper;
using WinAnprSqe.Models;
using WinAnprSqe.Server;

namespace WinAnprSqe
{
    public partial class MainForm : Form
    {
        private readonly ApiServer _apiServerStandart;
        private readonly ApiServer _apiServerTec;
        public BindingList<CarInlineViewModel> CarsStandart = new BindingList<CarInlineViewModel>();
        public BindingList<CarInlineViewModel> CarsTec = new BindingList<CarInlineViewModel>();

        public MainForm()
        {
            InitializeComponent();
            
            var apiUrlStd = ConfigurationManager.AppSettings["ApiUrlStandart"];
            var apiUrlTec = ConfigurationManager.AppSettings["ApiUrlTec"];
            
            _apiServerStandart = new ApiServer(apiUrlStd, this);
            _apiServerStandart.Start();

            _apiServerTec = new ApiServer(apiUrlTec, this);
            _apiServerTec.Start();

            Text = ConfigurationManager.AppSettings["WindowTitle"];
            PrinterHelper.PrinterName = ConfigurationManager.AppSettings["PrinterName"];
            PrinterHelper.PhoneNumber = ConfigurationManager.AppSettings["PhoneNumber"];
            PrinterHelper.Text1 = ConfigurationManager.AppSettings["Text1"];
            PrinterHelper.Text2 = ConfigurationManager.AppSettings["Text2"];
            PrinterHelper.Text3 = ConfigurationManager.AppSettings["Text3"];
            PrinterHelper.Text4 = ConfigurationManager.AppSettings["Text4"];
            
            DataGridMonitor.DataSource = CarsStandart;
            dataGridViewTec.DataSource = CarsTec;
            
            // Standart Table
            DataGridMonitor.Columns["ServiceName"].HeaderText = "Услуга";
            DataGridMonitor.Columns["LicensePlate"].HeaderText = "Номер машины";
            DataGridMonitor.Columns["Talon"].HeaderText = "Талон";
            DataGridMonitor.Columns["Date"].HeaderText = "Дата";
            
            DataGridMonitor.Columns["ServiceName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            DataGridMonitor.Columns["LicensePlate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            DataGridMonitor.Columns["Talon"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            DataGridMonitor.Columns["Date"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            
            // VIP Table (TEC)
            dataGridViewTec.Columns["ServiceName"].HeaderText = "Услуга";
            dataGridViewTec.Columns["LicensePlate"].HeaderText = "Номер машины";
            dataGridViewTec.Columns["Talon"].HeaderText = "Талон";
            dataGridViewTec.Columns["Date"].HeaderText = "Дата";
            
            dataGridViewTec.Columns["ServiceName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewTec.Columns["LicensePlate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewTec.Columns["Talon"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewTec.Columns["Date"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void TableAnpr_FormClosed(object sender, FormClosedEventArgs e)
        {
            _apiServerStandart.Stop();
            _apiServerTec.Stop();
        }
        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Scroll:
                    HandleF1KeyAsync();
                    return true;
                case Keys.Pause:
                    HandleF2KeyAsync();
                    return true;
                default:
                    return base.ProcessCmdKey(ref msg, keyData); // Call base method for unhandled keys
            }
        }

        private async void HandleF1KeyAsync()
        {
            try
            {
                await _apiServerStandart.AddToQueueAsync("Standart", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void HandleF2KeyAsync()
        {
            try
            {
                await _apiServerTec.AddToQueueAsync("TEC", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_print_Click(object sender, EventArgs e)
        {
            PrinterHelper.NewCar = new CarInlineViewModel
            {
                ServiceName = "Test service",
                LicensePlate = "01KG000AA",
                Date = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                Talon = "A10"
            };
            
            PrinterHelper.Print();
        }
    }
}