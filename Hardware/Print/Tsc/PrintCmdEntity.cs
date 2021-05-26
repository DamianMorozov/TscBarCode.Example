// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Hardware.Zpl;

namespace Hardware.Print.Tsc
{
    public class PrintCmdEntity : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised([CallerMemberName] string caller = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }

        #endregion

        #region Public properties

        private string _textPrepare;
        public string TextPrepare
        {
            get => _textPrepare;
            set
            {
                _textPrepare = value;
                OnPropertyRaised();
            }
        }
        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyRaised();
            }
        }
        private PrintControlEntity PrintControl { get; set; }
        private Exception _exception;
        public Exception Exception
        {
            get => _exception;
            set
            {
                _exception = value;
                OnPropertyRaised();
            }
        }

        #endregion

        #region Constructor and destructor

        public PrintCmdEntity(PrintControlEntity printControl)
        {
            PrintControl = printControl;
        }

        #endregion

        #region Public and private methods

        public void SendCustom(bool isClose, string cmd, bool isClearBuffer)
        {
            try
            {
                Exception = null;
                Text = cmd;
                PrintControl.Open();
                switch (PrintControl.Interface)
                {
                    case PrintInterface.Usb:
                        break;
                    case PrintInterface.Ethernet:
                        if (PrintControl.IsOpen && isClearBuffer)
                            PrintControl.TscEthernet.clearbuffer();
                        if (PrintControl.IsOpen && !string.IsNullOrEmpty(cmd))
                            PrintControl.TscEthernet.sendcommand(cmd);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(PrintInterface), PrintControl.Interface, null);
                }
                if (isClose)
                    PrintControl.Close();
            }
            catch (Exception ex)
            {
                Exception = ex;
                throw;
            }
        }

        public void ConvertZpl(bool isUsePicReplace)
        {
            Text = ZplPipeUtils.ToCodePoints(TextPrepare);
            if (isUsePicReplace)
            {
                Text = Text.Replace("[EAC_107x109_090]", ZplSamples.GetEac);
                Text = Text.Replace("[FISH_94x115_000]", ZplSamples.GetFish);
                Text = Text.Replace("[TEMP6_116x113_090]", ZplSamples.GetTemp6);
            }
        }

        public void Calibrate(bool isClose, bool isClearBuffer)
        {
            SendCustom(isClose, "GAPDETECT", isClearBuffer);
        }

        public void SetGap(bool isClose, bool isClearBuffer, double gapSize = 3.5, double gapOffset = 0.0)
        {
            var strGapSize = $"{gapSize}".Replace(',', '.');
            var strGapOffset = $"{gapOffset}".Replace(',', '.');
            SendCustom(isClose, $"GAP {strGapSize} mm, {strGapOffset} mm", isClearBuffer);
        }

        public void ClearBuffer(bool isClose)
        {
            PrintControl.Open();
            PrintControl.TscEthernet.clearbuffer();
            if (isClose)
                PrintControl.Close();
        }

        public void SetCutter(bool isClose, int value, bool isClearBuffer)
        {
            if (value >= 0)
                SendCustom(isClose, $"SET CUTTER {value}", isClearBuffer);
        }

        public void PrintTest(bool isClose)
        {
            PrintControl.Open();

            PrintControl.TscEthernet.clearbuffer();

            PrintControl.TscEthernet.barcode("100", "200", "128", "100", "1", "0", "3", "3", "123456789");
            PrintControl.TscEthernet.printerfont("100", "100", "3", "0", "1", "1", "Printer Font Test");
            PrintControl.TscEthernet.sendcommand("BOX 50,50,500,400,3\n");
            PrintControl.TscEthernet.printlabel("1", "1");

            if (isClose)
                PrintControl.Close();
        }

        public void Feed(bool isClose, bool isClearBuffer, PrintDpi dpi, int mm)
        {
            int value;
            switch (dpi)
            {
                case PrintDpi.Dpi100:
                    value = 4 * mm;
                    break;
                case PrintDpi.Dpi200:
                    value = 8 * mm;
                    break;
                case PrintDpi.Dpi300:
                    value = 12 * mm;
                    break;
                case PrintDpi.Dpi400:
                    value = 16 * mm;
                    break;
                case PrintDpi.Dpi500:
                    value = 20 * mm;
                    break;
                case PrintDpi.Dpi600:
                    value = 24 * mm;
                    break;
                case PrintDpi.Dpi700:
                    value = 28 * mm;
                    break;
                case PrintDpi.Dpi800:
                    value = 32 * mm;
                    break;
                case PrintDpi.Dpi900:
                    value = 36 * mm;
                    break;
                case PrintDpi.Dpi1000:
                    value = 40 * mm;
                    break;
                case PrintDpi.Dpi1100:
                    value = 44 * mm;
                    break;
                case PrintDpi.Dpi1200:
                    value = 48 * mm;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dpi), dpi, null);
            }
            if (value > 0)
                SendCustom(isClose, $"FEED {value}", isClearBuffer);
        }

        #endregion
    }
}
