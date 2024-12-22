using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Model = EmployeeApp.Model;

namespace EmployeeApp.ViewModel
{
    public class OrderTicketsViewModel : ViewModelBase
    {
        public OrderTicketsViewModel(Window window) 
        : base(window)
        {
        }

        private Model.Order order;
        public Model.Order Order
        {
            get => order;
            set
            {
                order = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Model.Ticket> tickets = new ObservableCollection<Model.Ticket>();
        public ObservableCollection<Model.Ticket> Tickets
        {
            get => tickets;
            set
            {
                tickets = value;
                OnPropertyChanged();
            }
        }

    }
}
