using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Hardware.Utils;

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
        private Speed _speed;
        public string Speed
        {
            get => Convert.ToString((int)_speed);
            set
            {
                if (int.TryParse(value, out int temp))
                {
                    if (temp >= 0 && temp <= 12)
                        _speed = (Speed)temp;
                    else
                        _speed = (Speed)int.Parse(SpeedDefault);
                }
                OnPropertyRaised();
            }
        }

        public string DensityDefault => "6";
        private Density _density;
        public string Density
        {
            get => Convert.ToString((int)_density);
            set
            {
                if (int.TryParse(value, out int temp))
                {
                    if (temp >= 0 && temp <= 15)
                        _density = (Density)temp;
                    else
                        _density = (Density)int.Parse(DensityDefault);
                }
                OnPropertyRaised();
            }
        }

        public string SensorDefault => "0";
        private Sensor _sensor;
        public string Sensor
        {
            get => Convert.ToString((int)_sensor);
            set
            {
                if (int.TryParse(value, out int temp))
                {
                    if (temp >= 0 && temp <= 1)
                        _sensor = (Sensor)temp;
                    else
                        _sensor = (Sensor)int.Parse(SensorDefault);
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

        public PrintSetupEntity(LabelSize size)
        {
            switch (size)
            {
                case LabelSize.Size40x60:
                    Width = "40";
                    Height = "60";
                    break;
                case LabelSize.Size60x150:
                    Width = "60";
                    Height = "150";
                    break;
                case LabelSize.Size60x90:
                    Width = "60";
                    Height = "90";
                    break;
                case LabelSize.Size60x100:
                    Width = "60";
                    Height = "100";
                    break;
                case LabelSize.Size80x100:
                    if (CultureInfo.CurrentCulture.Name.Equals("ru-RU"))
                    {
                        Width = "81,95";
                        Height = "100,10";
                    }
                    else
                    {
                        Width = "81.95";
                        Height = "100.10";
                    }
                    break;
                case LabelSize.Size100x100:
                    Width = "100";
                    Height = "100";
                    break;
                case LabelSize.Size100x110:
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