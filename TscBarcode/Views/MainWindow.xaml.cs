// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Windows;
using Hardware.Print;
using Hardware.Print.Tsc;
using Hardware.Zpl;

namespace TscBarcode.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Public and private fields and properties

        private PrintControlEntity PrintControl { get; set; }

        #endregion

        #region Constructor and destructor

        public MainWindow()
        {
            InitializeComponent();

            var context = FindResource("ViewModelProgramSettings");
            if (context is PrintControlEntity printControl)
            {
                PrintControl = printControl;
                if (string.IsNullOrEmpty(PrintControl.IpAddress))
                    PrintControl.IpAddress = "192.168.6.132";
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ButtonClose_Click(sender, null);
        }

        #endregion

        #region Public and private methods

        private void ButtonUsb_Click(object sender, RoutedEventArgs e)
        {
            //string WT1 = "TSC Printers";
            //string B1 = "20080101";
            //byte[] result_unicode = System.Text.Encoding.GetEncoding("utf-16").GetBytes("unicode test");
            //byte[] result_utf8 = System.Text.Encoding.UTF8.GetBytes("TEXT 40,620,\"ARIAL.TTF\",0,12,12,\"utf8 test Wörter auf Deutsch\"");

            //byte status = TscLib.usbportqueryprinter();//0 = idle, 1 = head open, 16 = pause, following <ESC>!? command of TSPL manual
            //TSCSDK.usb. openport("usb");
            //TscLib.sendcommand("SIZE 100 mm, 120 mm");
            //TscLib.sendcommand("SPEED 4");
            //TscLib.sendcommand("DENSITY 12");
            //TscLib.sendcommand("DIRECTION 1");
            //TscLib.sendcommand("SET TEAR ON");
            //TscLib.sendcommand("CODEPAGE UTF-8");
            //TscLib.clearbuffer();
            //TscLib.downloadpcx("UL.PCX", "UL.PCX");
            //TscLib.windowsfont(40, 490, 48, 0, 0, 0, "Arial", "Windows Font Test");
            //TscLib.windowsfontUnicode(40, 550, 48, 0, 0, 0, "Arial", result_unicode);
            //TscLib.sendcommand("PUTPCX 40,40,\"UL.PCX\"");
            //TscLib.sendBinaryData(result_utf8, result_utf8.Length);
            //TscLib.barcode("40", "300", "128", "80", "1", "0", "2", "2", B1);
            //TscLib.printerfont("40", "440", "0", "0", "15", "15", WT1);
            //TscLib.printlabel("1", "1");
            //TscLib.closeport();
        }

        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            PrintControl.Open();
        }

        private void ButtonCmdCalibrate_Click(object sender, RoutedEventArgs e)
        {
            PrintControl.Cmd.Calibrate(false, PrintControl.IsClearBuffer);
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            PrintControl.Close();
        }

        private void ButtonCmdSendCustom_Click(object sender, RoutedEventArgs e)
        {
            PrintControl.Cmd.SendCustom(false, PrintControl.Cmd.Text, PrintControl.IsClearBuffer);
        }

        private void ButtonCmdConvertZpl_Click(object sender, RoutedEventArgs e)
        {
            PrintControl.Cmd.ConvertZpl(true);
        }

        private void ButtonCmdSetCutter_Click(object sender, RoutedEventArgs e)
        {
            PrintControl.Cmd.SetCutter(false, PrintControl.CutterValue, PrintControl.IsClearBuffer);
        }

        private void ButtonCmdPrintTest_Click(object sender, RoutedEventArgs e)
        {
            PrintControl.Cmd.PrintTest(true);
        }

        private void ButtonCmdClearBuffer_Click(object sender, RoutedEventArgs e)
        {
            PrintControl.Cmd.ClearBuffer(true);
        }

        private void ButtonPrintSetupReset_Click(object sender, RoutedEventArgs e)
        {
            PrintControl.Setup(PrintLabelSize.Size80x100, true);
        }

        private void ButtonPrintSetup_Click(object sender, RoutedEventArgs e)
        {
            PrintControl.Setup(PrintControl.Size, true);
        }

        private void ButtonFeed_Click(object sender, RoutedEventArgs e)
        {
            PrintControl.Cmd.Feed(true, PrintControl.IsClearBuffer, PrintControl.Dpi, PrintControl.FeedMm);
        }

        #endregion

        #region Public and private methods - ZPL

        public void ButtonGetZpl1_OnClick(object sender, RoutedEventArgs e)
        {
            PrintControl.Cmd.TextPrepare = ZplSamples.GetSample1;
            PrintControl.Cmd.ConvertZpl(true);
        }

        public void ButtonGetZpl2_OnClick(object sender, RoutedEventArgs e)
        {
            PrintControl.Cmd.TextPrepare = ZplSamples.GetSample2;
            PrintControl.Cmd.ConvertZpl(true);
        }

        public void ButtonGetZpl3_OnClick(object sender, RoutedEventArgs e)
        {
            PrintControl.Cmd.TextPrepare = ZplSamples.GetSample3;
            PrintControl.Cmd.ConvertZpl(true);
        }

        public void ButtonGetZplFull_OnClick(object sender, RoutedEventArgs e)
        {
            PrintControl.Cmd.TextPrepare = ZplSamples.GetSampleFull;
            PrintControl.Cmd.ConvertZpl(true);
        }

        #endregion
    }
}
