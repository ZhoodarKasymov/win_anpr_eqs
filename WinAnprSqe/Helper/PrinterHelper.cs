using System;
using System.Drawing;
using System.Drawing.Printing;
using WinAnprSqe.Models;

namespace WinAnprSqe.Helper
{
    public static class PrinterHelper
    {
        public static CarInlineViewModel NewCar;
        public static string PrinterName;
        
        public static void Print()
        {
            var print = new PrintDocument();
            print.PrintPage += PrintPageHandler;
            
            if(!string.IsNullOrEmpty(PrinterName))
                print.PrinterSettings.PrinterName = PrinterName;
            
            print.Print();
            NewCar = null;
        }
        
        private static void PrintPageHandler(object sender, PrintPageEventArgs e)
        {
            if(NewCar == null) return;
            
            var g = e.Graphics;
            var regularFont = new Font("Arial", 8);
            var boldFont = new Font("Arial", 9, FontStyle.Bold);
            
            var logo = Image.FromFile("logo.png");
            
            var receiptContent = "\u226aКыргызкөмур\u226b \n" +
                                    "мамлекеттик ишканасы\n" +
                                    "Кезектик номурунуз\n" +
                                    $"{NewCar.Talon}\n" +
                                    $"{NewCar.LicensePlate}\n" +
                                    $"\u25d4 {DateTime.Now.ToString("HH:mm")}\n" +
                                    "------------------------------------------------------------\n" +
                                    "СУРАНЫЧ\n" +
                                    "Кезектик тартипти сактап чакыруу\nменен кириңиз\n" +
                                    "------------------------------------------------------------\n" +
                                    $"Талон берилди {DateTime.Now.ToString("dd/MM/yyyy")} жыл\n" + 
                                    "Электрондук кезек бузуу учурунда кубө болсоңуз \nтөмөнку номерге кабарлаңыз\n" +
                                    "\u2706 0312 055 350";

            var topMargin = 5;

            var yPos = topMargin + 20;
            var lines = receiptContent.Split('\n');
            var lineCount = 1;
            
            // Print the logo
            g.DrawImage(logo, topMargin + 40, yPos - 8, 25, 25);
            
            foreach (var line in lines)
            {
                yPos += 17;
                
                switch (lineCount)
                {
                    case 1:
                        g.DrawString(line, new Font("Arial", 6, FontStyle.Bold), Brushes.Black, topMargin + 15, yPos);
                        break;
                    case 2:
                        g.DrawString(line, new Font("Arial", 6, FontStyle.Bold), Brushes.Black, topMargin + 8, yPos - 10);
                        break;
                    case 3:
                        g.DrawString(line, new Font("Arial", 6, FontStyle.Bold), Brushes.Black, topMargin + 9, yPos);
                        break;
                    case 4:
                        g.DrawString(line, new Font("Arial", 28, FontStyle.Bold), Brushes.Black, topMargin + 23, yPos + 2);
                        break;
                    case 5:
                        g.DrawString(line, new Font("Arial", 8, FontStyle.Bold), Brushes.Black, topMargin + 17, yPos + 25);
                        break;
                    case 6:
                        g.DrawString(line, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, topMargin + 30, yPos + 20);
                        break;
                    case 12:
                        g.DrawString(line, new Font("Arial", 5, FontStyle.Bold), Brushes.Black, topMargin + 15, yPos - 5);
                        break;
                    case 13:
                        g.DrawString(line, new Font("Arial", 5, FontStyle.Bold), Brushes.Black, topMargin, yPos);
                        break;
                    case 14:
                        g.DrawString(line, new Font("Arial", 5, FontStyle.Bold), Brushes.Black, topMargin + 15, yPos);
                        break;
                    case 8:
                        g.DrawString(line, new Font("Arial", 6, FontStyle.Bold), Brushes.Black, topMargin + 25, yPos + 5);
                        break;
                    case 10:
                        g.DrawString(line, new Font("Arial", 5, FontStyle.Bold), Brushes.Black, topMargin + 21, yPos - 13);
                        break;
                    case 9:
                        g.DrawString(line, new Font("Arial", 5, FontStyle.Bold), Brushes.Black, topMargin, yPos - 3);
                        break;
                    case 11:
                        g.DrawString(line, new Font("Arial", 5, FontStyle.Regular), Brushes.Black, topMargin, yPos - 20);
                        break;
                    case 15:
                        g.DrawString(line, new Font("Arial", 6, FontStyle.Regular), Brushes.Black, topMargin + 23, yPos + 5);
                        break;
                    case 7:
                        g.DrawString(line, new Font("Arial", 5, FontStyle.Regular), Brushes.Black, topMargin, yPos + 15);
                        break;
                    default:
                        g.DrawString(line, new Font("Arial", 5, FontStyle.Regular), Brushes.Black, topMargin, yPos + 10);
                        break;
                }

                lineCount++;
            }

            e.HasMorePages = false;
        }
    }
}