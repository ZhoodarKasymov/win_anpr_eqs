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
        public static string PhoneNumber = string.Empty;
        public static string Text1 = string.Empty;
        public static string Text2 = string.Empty;
        public static string Text3 = string.Empty;
        public static string Text4 = string.Empty;
        
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
            var leading = 4;
            float lineHeight;

            using (var font14 = new Font("Courier New", 14))
            {
                lineHeight = font14.GetHeight() + leading;
            }

            float startX = 0;
            float startY = leading;
            float offset = 0;

            var formatLeft = new StringFormat(StringFormatFlags.NoClip);
            var formatCenter = new StringFormat(formatLeft);
            var formatRight = new StringFormat(formatLeft);

            formatCenter.Alignment = StringAlignment.Center;
            formatRight.Alignment = StringAlignment.Far;
            formatLeft.Alignment = StringAlignment.Near;

            var layoutSize = new SizeF(280.0f - offset * 2, lineHeight);
            
            var receiptContent = $"\u226a{Text1}\u226b \n" +
                                    $"{Text2}\n" +
                                    "Кезектик номурунуз\n" +
                                    $"{NewCar.Talon}\n" +
                                    $"{NewCar.LicensePlate}\n" +
                                    $"{DateTime.Now.ToString("HH:mm")}\n" +
                                    "".PadRight(64,'-') + "\n" +
                                    "СУРАНЫЧ\n" +
                                    $"{Text3}\n" +
                                    "".PadRight(64,'-') + "\n" +
                                    $"Талон берилди {DateTime.Now.ToString("dd/MM/yyyy")} жыл\n" + 
                                    $"{Text4}\n" +
                                    "\u2706" + PhoneNumber;

            var lines = receiptContent.Split('\n');
            var lineCount = 1;
            
            // Calculate the center position
            var paperWidth = e.PageSettings.PaperSize.Width;
            var centerPos = (paperWidth - e.MarginBounds.Width) / 2;
            
            centerPos -= 50;

            using (var logo = Image.FromFile("logo.png"))
            {
                // Print the logo
                g.DrawImage(logo, centerPos + 75, 0, 25, 25);
            }

            var bold10 = new Font("Arial", 10, FontStyle.Bold);
            var bold30 = new Font("Arial", 30, FontStyle.Bold);
            var bold15 = new Font("Arial", 15, FontStyle.Bold);
            var bold18 = new Font("Arial", 8, FontStyle.Bold);
            var regular9 = new Font("Arial", 9, FontStyle.Regular);
            
            foreach (var line in lines)
            {
                offset += lineHeight;
                var layout = new RectangleF(new PointF(startX, startY + offset), layoutSize);
                
                switch (lineCount)
                {
                    case 1:
                        g.DrawString(line, bold10, Brushes.Black, layout, formatCenter);
                        break;
                    case 2:
                        g.DrawString(line, bold10, Brushes.Black, layout, formatCenter);
                        break;
                    case 3:
                        g.DrawString(line, bold10, Brushes.Black, layout, formatCenter);
                        break;
                    case 4:
                        g.DrawString(line, bold30, Brushes.Black, layout, formatCenter);
                        offset += 16;
                        break;
                    case 5:
                        g.DrawString(line, bold15, Brushes.Black, layout, formatCenter);
                        break;
                    case 6:
                        g.DrawString(line, bold15, Brushes.Black, layout, formatCenter);
                        break;
                    case 11:
                        g.DrawString(line, bold18, Brushes.Black, layout, formatCenter);
                        break;
                    case 12:
                        g.DrawString(line, bold18, Brushes.Black, layout, formatCenter);
                        break;
                    case 8:
                        g.DrawString(line, bold10, Brushes.Black, layout, formatCenter);
                        break;
                    case 9:
                        g.DrawString(line, bold18, Brushes.Black, layout, formatCenter);
                        break;
                    case 10:
                        g.DrawString(line, regular9, Brushes.Black, layout, formatCenter);
                        break;
                    case 13:
                        g.DrawString(line, bold10, Brushes.Black, layout, formatCenter);
                        break;
                    case 7:
                        g.DrawString(line, regular9, Brushes.Black, layout, formatCenter);
                        break;
                    default:
                        g.DrawString(line, bold18, Brushes.Black, layout, formatCenter);
                        break;
                }
            
                lineCount++;
            }
            
            bold10.Dispose(); bold15.Dispose(); bold30.Dispose(); 
            bold18.Dispose(); regular9.Dispose();

            e.HasMorePages = false;
        }
    }
}