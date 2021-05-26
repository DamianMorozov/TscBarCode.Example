// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Hardware.Print.Tsc
{
    public class PrintControlEntity : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised([CallerMemberName] string caller = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }

        #endregion

        #region Public properties

        private PrintInterface _interface;
        public PrintInterface Interface
        {
            get => _interface;
            set
            {
                _interface = value;
                OnPropertyRaised();
            }
        }
        private PrintCmdEntity _cmd;
        public PrintCmdEntity Cmd
        {
            get => _cmd;
            set
            {
                _cmd = value;
                OnPropertyRaised();
            }
        }
        private string _ipAddress;
        public string IpAddress
        {
            get => _ipAddress;
            set
            {
                _ipAddress = value;
                OnPropertyRaised();
            }
        }
        private int _port;
        public int Port
        {
            get => _port;
            set
            {
                _port = value;
                OnPropertyRaised();
            }
        }
        private PrintLabelSize _size;
        public PrintLabelSize Size
        {
            get => _size;
            set
            {
                _size = value;
                OnPropertyRaised();
            }
        }
        private PrintDpi _dpi;
        public PrintDpi Dpi
        {
            get => _dpi;
            set
            {
                _dpi = value;
                OnPropertyRaised();
            }
        }
        private ushort _feedMm;
        public ushort FeedMm
        {
            get => _feedMm;
            set
            {
                _feedMm = value;
                OnPropertyRaised();
            }
        }
        private TSCSDK.ethernet _tscEthernet;
        public TSCSDK.ethernet TscEthernet
        {
            get => _tscEthernet;
            set
            {
                _tscEthernet = value;
                OnPropertyRaised();
            }
        }
        private PrintSetupEntity _printerSetup;
        public PrintSetupEntity PrinterSetup
        {
            get => _printerSetup;
            set
            {
                _printerSetup = value;
                OnPropertyRaised();
            }
        }
        private bool _isOpen;
        public bool IsOpen
        {
            get => _isOpen;
            private set
            {
                _isOpen = value;
                OnPropertyRaised();
            }
        }
        
        private bool _isStatusNormal;
        public bool IsStatusNormal
        {
            get => _isStatusNormal;
            private set
            {
                _isStatusNormal = value;
                OnPropertyRaised();
            }
        }
        private int _cutterValue;
        public int CutterValue
        {
            get => _cutterValue;
            set
            {
                _cutterValue = value;
                OnPropertyRaised();
            }
        }
        private bool _isClearBuffer;
        public bool IsClearBuffer
        {
            get => _isClearBuffer;
            set
            {
                _isClearBuffer = value;
                OnPropertyRaised();
            }
        }
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

        #region Constructor

        public PrintControlEntity() : this(PrintInterface.Ethernet, "")
        {
            //
        }
        
        public PrintControlEntity(PrintInterface @interface, string ipAddress = "", int port = 9100, PrintLabelSize size = PrintLabelSize.Size80x100, PrintDpi dpi = PrintDpi.Dpi300)
        {
            IpAddress = ipAddress;
            Port = port;
            Interface = @interface;
            Size = size;
            Dpi = dpi;
            PrinterSetup = new PrintSetupEntity(Size);
            Cmd = new PrintCmdEntity(this);
            IsClearBuffer = true;
        }

        #endregion

        #region Public and private methods - Base

        public void Open()
        {
            try
            {
                Exception = null;
                switch (Interface)
                {
                    case PrintInterface.Usb:
                        break;
                    case PrintInterface.Ethernet:
                        if (TscEthernet == null)
                            TscEthernet = new TSCSDK.ethernet();
                        if (!IsOpen && !string.IsNullOrEmpty(IpAddress) && Port > 0)
                        {
                            TscEthernet.openport(IpAddress, Port);
                            IsOpen = true;
                            IsStatusNormal = TscEthernet != null && Equals(GetStatusAsStringEng(), "Normal");
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(Interface), Interface, null);
                }
            }
            catch (Exception ex)
            {
                Exception = ex;
                throw;
            }
        }

        public void Setup(PrintLabelSize size, bool isResetGap)
        {
            Open();

            PrinterSetup = new PrintSetupEntity(size);
            if (isResetGap)
                Cmd.SetGap(false, true);

            switch (Interface)
            {
                case PrintInterface.Usb:
                    break;
                case PrintInterface.Ethernet:
                    if (IsOpen)
                        TscEthernet?.setup(
                            PrinterSetup.Width,
                            PrinterSetup.Height,
                            PrinterSetup.Speed,
                            PrinterSetup.Density,
                            PrinterSetup.Sensor,
                            PrinterSetup.Vertical,
                            PrinterSetup.Offset);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Interface), Interface, null);
            }
        }

        public void Close()
        {
            try
            {
                Exception = null;
                switch (Interface)
                {
                    case PrintInterface.Usb:
                        break;
                    case PrintInterface.Ethernet:
                        if (IsOpen)
                            TscEthernet?.closeport();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(Interface), Interface, null);
                }
                IsOpen = false;
                IsStatusNormal = false;
            }
            catch (Exception ex)
            {
                Exception = ex;
                throw;
            }
        }

        #endregion

        #region Public and private methods

        public PrintStatus GetStatusAsEnum(byte? value)
        {
            switch (value)
            {
                // Normal
                case (byte)0x00:
                    return (PrintStatus)0x00;
                // Head opened
                case (byte)0x01:
                    return (PrintStatus)0x01;
                // Paper Jam
                case (byte)0x02:
                    return (PrintStatus)0x02;
                // Paper Jam and head opened
                case (byte)0x03:
                    return (PrintStatus)0x03;
                // Out of paper
                case (byte)0x04:
                    return (PrintStatus)0x04;
                // Out of paper and head opened
                case (byte)0x05:
                    return (PrintStatus)0x05;
                // Out of ribbon
                case (byte)0x08:
                    return (PrintStatus)0x08;
                // Out of ribbon and head opened
                case (byte)0x09:
                    return (PrintStatus)0x09;
                // Out of ribbon and paper jam
                case (byte)0x0A:
                    return (PrintStatus)0x0A;
                // Out of ribbon, paper jam and head opened
                case (byte)0x0B:
                    return (PrintStatus)0x0B;
                // Out of ribbon and out of paper
                case (byte)0x0C:
                    return (PrintStatus)0x0C;
                // Out of ribbon, out of paper and head opened
                case (byte)0x0D:
                    return (PrintStatus)0x0D;
                // Pause
                case (byte)0x10:
                    return (PrintStatus)0x10;
                // Printing
                case (byte)0x20:
                    return (PrintStatus)0x20;
            }
            return PrintStatus.HundredTwentyEight;
        }

        public PrintStatus GetStatusAsEnum()
        {
            return GetStatusAsEnum(TscEthernet?.printerstatus());
        }

        public string GetStatusAsStringRus(byte? value)
        {
            switch (value)
            {
                // Normal
                case (byte)0x00:
                    return "Нормальный";
                // Head opened
                case (byte)0x01:
                    return "Голова открыта";
                // Paper Jam
                case (byte)0x02:
                    return "Замятие бумаги";
                // Paper Jam and head opened
                case (byte)0x03:
                    return "Замятие бумаги и открыта голова";
                // Out of paper
                case (byte)0x04:
                    return "Нет бумаги";
                // Out of paper and head opened
                case (byte)0x05:
                    return "Нет бумаги и голова открыта";
                // Out of ribbon
                case (byte)0x08:
                    return "Нет ленты";
                // Out of ribbon and head opened
                case (byte)0x09:
                    return "Нет ленты и голова открыта";
                // Out of ribbon and paper jam
                case (byte)0x0A:
                    return "Закончилась лента и застряла бумага";
                // Out of ribbon, paper jam and head opened
                case (byte)0x0B:
                    return "Закончилась лента, застряла бумага и голова открыта";
                // Out of ribbon and out of paper
                case (byte)0x0C:
                    return "Нет ленты и нет бумаги";
                // Out of ribbon, out of paper and head opened
                case (byte)0x0D:
                    return "Нет ленты, нет бумаги и голова открыта";
                // Pause
                case (byte)0x10:
                    return "Пауза";
                // Printing
                case (byte)0x20:
                    return "Печать";
            }
            return "Другая ошибка";
        }

        public string GetStatusAsStringRus()
        {
            return GetStatusAsStringRus(TscEthernet?.printerstatus());
        }

        public string GetStatusAsStringEng(byte? value)
        {
            switch (value)
            {
                // Normal
                case (byte)0x00:
                    return "Normal";
                // Head opened
                case (byte)0x01:
                    return "Head opened";
                // Paper Jam
                case (byte)0x02:
                    return "Paper Jam";
                // Paper Jam and head opened
                case (byte)0x03:
                    return "Paper Jam and head opened";
                // Out of paper
                case (byte)0x04:
                    return "Out of paper";
                // Out of paper and head opened
                case (byte)0x05:
                    return "Out of paper and head opened";
                // Out of ribbon
                case (byte)0x08:
                    return "Out of ribbon";
                // Out of ribbon and head opened
                case (byte)0x09:
                    return "Out of ribbon and head opened";
                // Out of ribbon and paper jam
                case (byte)0x0A:
                    return "Out of ribbon and paper jam";
                // Out of ribbon, paper jam and head opened
                case (byte)0x0B:
                    return "Out of ribbon, paper jam and head opened";
                // Out of ribbon and out of paper
                case (byte)0x0C:
                    return "Out of ribbon and out of paper";
                // Out of ribbon, out of paper and head opened
                case (byte)0x0D:
                    return "Out of ribbon, out of paper and head opened";
                // Pause
                case (byte)0x10:
                    return "Pause";
                // Printing
                case (byte)0x20:
                    return "Printing";
            }
            return "Other error";
        }

        public string GetStatusAsStringEng()
        {
            if (TscEthernet != null)
            {
                var st = TscEthernet.printerstatus();
                return GetStatusAsStringEng(st);
            }
            return GetStatusAsStringEng((byte)PrintStatus.HundredTwentyEight);
        }

        #endregion
    }
}