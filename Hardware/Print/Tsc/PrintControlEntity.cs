using Hardware.Utils;
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

        private string _cmd;
        public string Cmd
        {
            get => _cmd;
            set
            {
                _cmd = value;
                OnPropertyRaised();
            }
        }
        
        private LabelSize _size;
        public LabelSize Size
        {
            get => _size;
            set
            {
                _size = value;
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
        public bool IsOpen { get; private set; }
        public bool IsStatusNormal => TscEthernet != null && Equals(GetStatusAsStringEng(), "Normal");
        public int CutterValue { get; set; }
        public Interface Interface { get; set; }

        #endregion

        #region Constructor

        public PrintControlEntity() : this(Interface.Ethernet, "")
        {
            //
        }
        
        public PrintControlEntity(Interface @interface, string ipAddress = "", int port = 9100)
        {
            IpAddress = ipAddress;
            Port = port;
            Interface = @interface;
            Size = LabelSize.Size80x100;
            PrinterSetup = new PrintSetupEntity(Size);
            Setup(LabelSize.Size80x100, true);
        }

        #endregion

        #region Public and private methods - Base

        public void Open()
        {
            switch (Interface)
            {
                case Interface.Usb:
                    break;
                case Interface.Ethernet:
                    if (TscEthernet == null)
                        TscEthernet = new TSCSDK.ethernet();
                    if (!IsOpen && !string.IsNullOrEmpty(IpAddress) && Port > 0)
                    {
                        TscEthernet.openport(IpAddress, Port);
                        IsOpen = true;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Interface), Interface, null);
            }
        }

        public void Setup(LabelSize size, bool isResetGap)
        {
            Open();

            PrinterSetup = new PrintSetupEntity(size);
            if (isResetGap)
                SetGap(false, true);

            switch (Interface)
            {
                case Interface.Usb:
                    break;
                case Interface.Ethernet:
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
            IsOpen = false;
            switch (Interface)
            {
                case Interface.Usb:
                    break;
                case Interface.Ethernet:
                    if (IsOpen)
                        TscEthernet?.closeport();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Interface), Interface, null);
            }
        }

        public void SendCmd(bool isClose, string cmd, bool isClearBuffer)
        {
            Cmd = cmd;
            Open();
            switch (Interface)
            {
                case Interface.Usb:
                    break;
                case Interface.Ethernet:
                    if (IsOpen && isClearBuffer)
                        TscEthernet.clearbuffer();
                    if (IsOpen && !string.IsNullOrEmpty(cmd))
                        TscEthernet.sendcommand(cmd);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Interface), Interface, null);
            }
            if (isClose)
                Close();
        }

        #endregion

        #region Public and private methods

        public Status GetStatusAsEnum(byte? value)
        {
            switch (value)
            {
                // Normal
                case (byte)0x00:
                    return (Status)0x00;
                // Head opened
                case (byte)0x01:
                    return (Status)0x01;
                // Paper Jam
                case (byte)0x02:
                    return (Status)0x02;
                // Paper Jam and head opened
                case (byte)0x03:
                    return (Status)0x03;
                // Out of paper
                case (byte)0x04:
                    return (Status)0x04;
                // Out of paper and head opened
                case (byte)0x05:
                    return (Status)0x05;
                // Out of ribbon
                case (byte)0x08:
                    return (Status)0x08;
                // Out of ribbon and head opened
                case (byte)0x09:
                    return (Status)0x09;
                // Out of ribbon and paper jam
                case (byte)0x0A:
                    return (Status)0x0A;
                // Out of ribbon, paper jam and head opened
                case (byte)0x0B:
                    return (Status)0x0B;
                // Out of ribbon and out of paper
                case (byte)0x0C:
                    return (Status)0x0C;
                // Out of ribbon, out of paper and head opened
                case (byte)0x0D:
                    return (Status)0x0D;
                // Pause
                case (byte)0x10:
                    return (Status)0x10;
                // Printing
                case (byte)0x20:
                    return (Status)0x20;
            }
            return Status.HundredTwentyEight;
        }

        public Status GetStatusAsEnum()
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
            return GetStatusAsStringEng((byte)Status.HundredTwentyEight);
        }

        #endregion

        #region Public and private methods - Control

        public void Calibrate(bool isClose, bool isClearBuffer)
        {
            SendCmd(isClose, "GAPDETECT", isClearBuffer);
        }

        public void SetGap(bool isClose, bool isClearBuffer, double gapSize = 4.0, double gapOffset = 0.0)
        {
            var strGapSize = $"{gapSize}".Replace(',', '.');
            var strGapOffset = $"{gapOffset}".Replace(',', '.');
            SendCmd(isClose, $"GAP {strGapSize} mm, {strGapOffset} mm", isClearBuffer);
        }

        public void ClearBuffer(bool isClose)
        {
            SendCmd(isClose, string.Empty, true);
        }

        public void SetCutter(bool isClose, int value, bool isClearBuffer)
        {
            if (value >= 0)
                SendCmd(isClose, $"SET CUTTER {value}", isClearBuffer);
        }

        public void PrintTest(bool isClose)
        {
            Open();

            TscEthernet.clearbuffer();

            TscEthernet.barcode("100", "200", "128", "100", "1", "0", "3", "3", "123456789");
            TscEthernet.printerfont("100", "100", "3", "0", "1", "1", "Printer Font Test");
            TscEthernet.sendcommand("BOX 50,50,500,400,3\n");
            TscEthernet.printlabel("1", "1");

            if (isClose)
                Close();
        }

        #endregion
    }
}