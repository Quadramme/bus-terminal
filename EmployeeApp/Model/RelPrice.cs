using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApp.Model
{
    public class RelPrice : INotifyPropertyChanged
    {
        private decimal adultPrice;
        public decimal AdulPriceDecimal
        {

            get => adultPrice;
            set
            {
                adultPrice = value;
                OnPropertyChanged("AdultPriceDecimal");
                OnPropertyChanged("AdultPrice");
            }

        }
        public int AdultPrice
        {
            get => (int)adultPrice;
            set
            {
                AdulPriceDecimal = value;
            }
        }

        private decimal underagePrice;
        public decimal UnderagePriceDecimal
        {

            get => underagePrice;
            set
            {
                underagePrice = value;
                OnPropertyChanged("UnderagePriceDecimal");
                OnPropertyChanged("UnderagePrice");
            }

        }
        public int UnderagePrice
        {
            get => (int)underagePrice;
            set
            {
                UnderagePriceDecimal = value;
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