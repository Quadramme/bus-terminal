using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UserApp.ViewModel
{
    public class OrderResultViewModel : ViewModelBase
    {
        public OrderResultViewModel(Window window) : base(window)
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

        private ObservableCollection<Model.Ticket> tickets;
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
