using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace UserApp.Model
{
    public class Ticket : INotifyPropertyChanged
    {

        private int number;
        public int Number 
        {
            get => number;
            set
            {
                number = value;
                OnPropertyChanged();
            }
        }

        private string firstName;
        public string FirstName 
        {
            get => firstName; 
            set
            {
                firstName = value;
                OnPropertyChanged();
            }
        }

        private string middleName;
        public string MiddleName 
        {
            get => middleName;
            set
            {
                middleName = value;
                OnPropertyChanged();
            }
        }

        private string lastName;
        public string LastName 
        { 
            get => lastName; 
            set
            {
                lastName = value;
                OnPropertyChanged();
            }
        }

        public string FullName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(middleName)) 
                    return LastName + " " + FirstName;

                return LastName + " " + FirstName + " " + MiddleName;
            }
        }

        private bool? gender;
        public bool? Gender 
        { 
            get => gender; 
            set
            {
                gender = value;
                OnPropertyChanged();
            }
        }

        private int? seat;
        public int? Seat 
        {
            get => seat;
            set
            {
                seat = value;
                OnPropertyChanged();
            }
        }

        private DateTime? dob;
        public DateTime? DOB 
        {
            get => dob; 
            set
            {
                dob = value;
                OnPropertyChanged();
            }
        }

        private int luggage;
        public int Luggage 
        {
            get => luggage;
            set
            {
                luggage = value;
                OnPropertyChanged();
            }
        }

        private int maxLuggage;
        public int MaxLuggage
        {
            get => maxLuggage;
            set
            {
                maxLuggage = value;
                OnPropertyChanged();
            }
        }

        private int routeId;
        public int RouteId
        {
            get => routeId;
            set
            {
                routeId = value;
                OnPropertyChanged();
            }
        }

        private string id;
        public string Id
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged();
            }
        }

        private decimal price;
        public decimal Price
        { 
            get => price;
            set
            {
                price = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}