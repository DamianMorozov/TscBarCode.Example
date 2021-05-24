using System;
using System.Windows;
using Hardware.Print.Tsc;
using Hardware.Utils;
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
            ButtonEthernetClose_Click(sender, null);
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

        private void ButtonEthernetOpen_Click(object sender, RoutedEventArgs e)
        {
            PrintControl.Open();
        }

        private void ButtonCalibrate_Click(object sender, RoutedEventArgs e)
        {
            PrintControl.Calibrate(false, true);
        }

        private void ButtonEthernetClose_Click(object sender, RoutedEventArgs e)
        {
            PrintControl.Close();
        }

        private void ButtonEthernetSendCmd_Click(object sender, RoutedEventArgs e)
        {
            PrintControl.SendCmd(false, PrintControl.Cmd, true);
        }

        private void ButtonEthernetSetCutter_Click(object sender, RoutedEventArgs e)
        {
            PrintControl.SetCutter(false, PrintControl.CutterValue, true);
        }

        private void ButtonEthernetPrintTest_Click(object sender, RoutedEventArgs e)
        {
            PrintControl.PrintTest(false);
        }

        private void ButtonEthernetClearBuffer_Click(object sender, RoutedEventArgs e)
        {
            PrintControl.ClearBuffer(false);
        }

        private void ButtonEthernetPrinterSetupReset_Click(object sender, RoutedEventArgs e)
        {
            PrintControl.Setup(LabelSize.Size80x100, true);
        }

        private void ButtonEthernetPrinterSetup_Click(object sender, RoutedEventArgs e)
        {
            PrintControl.Setup(PrintControl.Size, true);
        }

        #endregion

        #region Public and private methods - ZPL

        public void ButtonGetZpl1_OnClick(object sender, RoutedEventArgs e)
        {
            PrintControl.Cmd = ZplPipeUtils.ToCodePoints(ZplSamples.GetSample1);
        }

        public void ButtonGetZpl2_OnClick(object sender, RoutedEventArgs e)
        {
            PrintControl.Cmd = ZplPipeUtils.ToCodePoints(ZplSamples.GetSample2);
        }

        public void ButtonGetZpl3_OnClick(object sender, RoutedEventArgs e)
        {
            PrintControl.Cmd = ZplPipeUtils.ToCodePoints(ZplSamples.GetSample3);
        }

        public void ButtonGetZplFull_OnClick(object sender, RoutedEventArgs e)
        {
            PrintControl.Cmd = ZplPipeUtils.ToCodePoints(ZplSamples.GetSampleFull);
            PrintControl.Cmd = PrintControl.Cmd.Replace("[EAC_107x109_090]", ZplSamples.GetEac);
            PrintControl.Cmd = PrintControl.Cmd.Replace("[FISH_94x115_000]", ZplSamples.GetFish);
            PrintControl.Cmd = PrintControl.Cmd.Replace("[TEMP6_116x113_090]", ZplSamples.GetTemp6);
        }

        #endregion
    }
}
