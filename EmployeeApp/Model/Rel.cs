using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApp.Model
{
    public class Rel : INotifyPropertyChanged
    {

        private bool enableMoveUp;
        public bool EnableMoveUp
        {
            get => enableMoveUp;
            set
            {
                enableMoveUp = value;
                OnPropertyChanged("EnableMoveUp");
            }
        }

        private bool enableMoveDown;
        public bool EnableMoveDown
        {
            get => enableMoveDown;
            set
            {
                enableMoveDown = value;
                OnPropertyChanged("EnableMoveDown");
            }
        }

        private int? destinationId;
        public int? DestinationId
        {
            get => destinationId;
            set
            {
                destinationId = value;
                OnPropertyChanged("DestinationId");

            }
        }

        private decimal? adultPrice;
        public decimal? AdultPrice
        {
            get => adultPrice;
            set
            {
                adultPrice = value;
                OnPropertyChanged("AdultPrice");
            }
        }

        private decimal? underagePrice;
        public decimal? UnderagePrice
        {
            get => underagePrice;
            set
            {
                underagePrice = value;
                OnPropertyChanged("UnderagePrice");
            }
        }

        private int? arrivalHour;
        public int? ArrivalHour
        {
            get => arrivalHour;
            set
            {
                arrivalHour = value;
                OnPropertyChanged("ArrivalHour");
                OnPropertyChanged("ArrivalTime");
            }
        }

        private byte? arrivalMinute;
        public byte? ArrivalMinute
        {
            get => arrivalMinute;
            set
            {
                arrivalMinute = value;
                OnPropertyChanged("ArrivalMinute");
                OnPropertyChanged("ArrivalTime");
            }
        }
        public DateTime? ArrivalTime
        {
            get
            {
                if (arrivalMinute == null || ArrivalHour == null)
                    return null;

                return new DateTime().AddHours((double)arrivalHour).AddMinutes((double)arrivalMinute);
            }
            set
            {

                if (value == null)
                {
                    ArrivalMinute = null;
                    ArrivalHour = null;
                }
                else
                {
                    ArrivalMinute = (byte)value?.Minute;
                    ArrivalHour = (int)value?.Hour;
                }

            }

        }

        private int? platform;
        public int? Platform
        {
            get => platform;
            set
            {
                platform = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}