// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Hardware.Print.Tsc
{
    public class PrintSetupEntity : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised([CallerMemberName] string caller = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }

        #endregion

        #region Public and private fields and properties

        public string WidthDefault => "80";
        private double _width;
        public string Width
        {
            get => Convert.ToString(_width, CultureInfo.InvariantCulture);
            set
            {
                if (double.TryParse(value, out double temp))
                {
                    if (temp >= 0 && temp <= 1000)
                        _width = temp;
                    else
                        _width = double.Parse(WidthDefault);
                }
                OnPropertyRaised();
            }
        }

        public string HeightDefault => "100";
        private double _height;
        public string Height
        {
            get => Convert.ToString(_height, CultureInfo.InvariantCulture);
            set
            {
                if (double.TryParse(value, out double temp))
                {
                    if (temp >= 0 && temp <= 1000)
                        _height = temp;
                    else
                        _height = double.Parse(HeightDefault);
                }
                OnPropertyRaised();
            }
        }

        public string SpeedDefault => "4";
        private PrintSpeed _speed;
        public string Speed
        {
            get => Convert.ToString((int)_speed);
            set
            {
                if (int.TryParse(value, out int temp))
                {
                    if (temp >= 0 && temp <= 12)
                        _speed = (PrintSpeed)temp;
                    else
                        _speed = (PrintSpeed)int.Parse(SpeedDefault);
                }
                OnPropertyRaised();
            }
        }

        public string DensityDefault => "6";
        private PrintDensity _density;
        public string Density
        {
            get => Convert.ToString((int)_density);
            set
            {
                if (int.TryParse(value, out int temp))
                {
                    if (temp >= 0 && temp <= 15)
                        _density = (PrintDensity)temp;
                    else
                        _density = (PrintDensity)int.Parse(DensityDefault);
                }
                OnPropertyRaised();
            }
        }

        public string SensorDefault => "0";
        private PrintSensor _sensor;
        public string Sensor
        {
            get => Convert.ToString((int)_sensor);
            set
            {
                if (int.TryParse(value, out int temp))
                {
                    if (temp >= 0 && temp <= 1)
                        _sensor = (PrintSensor)temp;
                    else
                        _sensor = (PrintSensor)int.Parse(SensorDefault);
                }
                OnPropertyRaised();
            }
        }

        public string VerticalDefault => "0";
        private int _vertical;
        public string Vertical
        {
            get => Convert.ToString(_vertical);
            set
            {
                if (int.TryParse(value, out int temp))
                {
                    if (temp >= 0 && temp <= 1000)
                        _vertical = temp;
                    else
                        _vertical = int.Parse(VerticalDefault);
                }
                OnPropertyRaised();
            }
        }

        public string OffsetDefault => "0";
        private int _offset;
        public string Offset
        {
            get => Convert.ToString(_offset);
            set
            {
                if (int.TryParse(value, out int temp))
                {
                    if (temp >= 0 && temp <= 1000)
                        _offset = temp;
                    else
                        _offset = int.Parse(VerticalDefault);
                }
                OnPropertyRaised();
            }
        }

        #endregion

        #region Constructor and destructor

        public PrintSetupEntity(PrintLabelSize size)
        {
            switch (size)
            {
                case PrintLabelSize.Size40x60:
                    Width = "40";
                    Height = "60";
                    break;
                case PrintLabelSize.Size60x150:
                    Width = "60";
                    Height = "150";
                    break;
                case PrintLabelSize.Size60x90:
                    Width = "60";
                    Height = "90";
                    break;
                case PrintLabelSize.Size60x100:
                    Width = "60";
                    Height = "100";
                    break;
                case PrintLabelSize.Size80x100:
                    if (CultureInfo.CurrentCulture.Name.Equals("ru-RU"))
                    {
                        Width = "83,00";
                        Height = "101,50";
                    }
                    else
                    {
                        Width = "83.00";
                        Height = "101.50";
                    }
                    break;
                case PrintLabelSize.Size100x100:
                    Width = "100";
                    Height = "100";
                    break;
                case PrintLabelSize.Size100x110:
                    Width = "100";
                    Height = "110";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(size), size, null);
            }
            Speed = SpeedDefault;
            Density = DensityDefault;
            Sensor = SensorDefault;
            Vertical = VerticalDefault;
            Offset = OffsetDefault;
        }

        #endregion
    }
}