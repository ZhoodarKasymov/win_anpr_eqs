﻿using System;
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
        private readonly ApiServer _apiServer;
        public BindingList<CarInlineViewModel> Cars = new BindingList<CarInlineViewModel>();

        public MainForm()
        {
            InitializeComponent();
            
            var apiUrl = ConfigurationManager.AppSettings["ApiUrl"];
            var printerName = ConfigurationManager.AppSettings["PrinterName"];
            
            _apiServer = new ApiServer(apiUrl, this);
            _apiServer.Start();
            
            PrinterHelper.PrinterName = printerName;
            
            DataGridMonitor.DataSource = Cars;
            
            DataGridMonitor.Columns["ServiceName"].HeaderText = "Услуга";
            DataGridMonitor.Columns["LicensePlate"].HeaderText = "Номер машины";
            DataGridMonitor.Columns["Talon"].HeaderText = "Талон";
            DataGridMonitor.Columns["Date"].HeaderText = "Дата";
            
            DataGridMonitor.Columns["ServiceName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            DataGridMonitor.Columns["LicensePlate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            DataGridMonitor.Columns["Talon"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            DataGridMonitor.Columns["Date"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void TableAnpr_FormClosed(object sender, FormClosedEventArgs e)
        {
            _apiServer.Stop();
        }

        private void button_print_Click(object sender, EventArgs e)
        {
            PrinterHelper.NewCar = new CarInlineViewModel
            {
                ServiceName = "Test service",
                LicensePlate = "01KG000AAA",
                Date = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                Talon = $"A1"
            };
            
            PrinterHelper.Print();
        }
    }
}