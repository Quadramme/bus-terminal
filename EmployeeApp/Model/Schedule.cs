using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApp.Model
{
    public class Schedule : INotifyPropertyChanged
    {

        private int schedule;
        public int ScheduleMask
        {
            get => schedule;
            set
            {
                schedule = value;
                OnPropertyChanged();
                OnPropertyChanged("Sunday");
                OnPropertyChanged("Monday");
                OnPropertyChanged("Tuesday");
                OnPropertyChanged("Wednesday");
                OnPropertyChanged("Thursday");
                OnPropertyChanged("Friday");
                OnPropertyChanged("Saturday");
            }
        }

        public bool Sunday
        {
            get => (schedule & 0b01000000) != 0;
            set
            {
                if (value)
                {
                    schedule |= 0b01000000;
                }
                else
                {
                    schedule &= 0b10111111;
                }
                OnPropertyChanged("Sunday");
            }
        }

        public bool Monday
        {
            get => (schedule & 0b00000001) != 0;
            set
            {
                if (value)
                {
                    schedule |= 0b00000001;
                }
                else
                {
                    schedule &= 0b11111110;
                }
                OnPropertyChanged("Monday");
            }
        }

        public bool Tuesday
        {
            get => (schedule & 0b00000010) != 0;
            set
            {
                if (value)
                {
                    schedule |= 0b00000010;
                }
                else
                {
                    schedule &= 0b11111101;
                }
                OnPropertyChanged("Tuesday");
            }
        }

        public bool Wednesday
        {
            get => (schedule & 0b00000100) != 0;
            set
            {
                if (value)
                {
                    schedule |= 0b00000100;
                }
                else
                {
                    schedule &= 0b11111011;
                }
                OnPropertyChanged("Wednesday");
            }
        }

        public bool Thursday
        {
            get => (schedule & 0b00001000) != 0;
            set
            {
                if (value)
                {
                    schedule |= 0b00001000;
                }
                else
                {
                    schedule &= 0b11110111;
                }
                OnPropertyChanged("Thusrday");
            }
        }

        public bool Friday
        {
            get => (schedule & 0b00010000) != 0;
            set
            {
                if (value)
                {
                    schedule |= 0b00010000;
                }
                else
                {
                    schedule &= 0b11101111;
                }
                OnPropertyChanged("Friday");
            }
        }

        public bool Saturday
        {
            get => (schedule & 0b00100000) != 0;
            set
            {
                if (value)
                {
                    schedule |= 0b00100000;
                }
                else
                {
                    schedule &= 0b11011111;
                }
                OnPropertyChanged("Saturday");
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