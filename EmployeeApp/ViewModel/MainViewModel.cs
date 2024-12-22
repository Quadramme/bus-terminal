using Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DTO = Interfaces.DTO;

namespace EmployeeApp.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(Window window, IRouteService routeService, IUserService userService) 
            : base(window)
        {
            vmSearchRoutes = new SearchRoutesViewModel(window, routeService);
            vmMakeOrder = new MakeOrderViewModel(window, routeService, userService);
            vmOrderResult = new OrderResultViewModel(window);
            vmUserOrders = new UserOrdersViewModel(window, routeService, userService);
            vmOrderTickets = new OrderTicketsViewModel(window);

            vmSearchRoutes.OnRouteSelected += vmMakeOrder.SelectRoute;
            vmSearchRoutes.OnRouteSelected += RouteSelected;

            vmMakeOrder.OnMakeOrder += OrderMade;
            vmMakeOrder.OnNoFreeSeats += NoFreeSeats;
            vmMakeOrder.OnRouteNotActive += OnRouteNotActive;

            vmUserOrders.OnOrderSelected += OrderSelected;
            
            SearchTabViewModel = vmSearchRoutes;
            UserTripsTabViewModel = vmUserOrders;

            vmSearchRoutes.ReloadDestinations();

            CmdBackToSearch = new RelayCommand(obj =>
            {
                SearchTabViewModel = vmSearchRoutes;
            });

            CmdBackToUserOrders = new RelayCommand(obj =>
            {
                UserTripsTabViewModel = vmUserOrders;
                vmUserOrders.ReloadOrders();
            });

        }

        public SearchRoutesViewModel vmSearchRoutes { get; private set; }
        public MakeOrderViewModel vmMakeOrder { get; private set; }
        public OrderResultViewModel vmOrderResult { get; private set; }
        public UserOrdersViewModel vmUserOrders { get; private set; }
        public OrderTicketsViewModel vmOrderTickets { get; private set; }


        private ViewModelBase userTripsTabViewModel;
        public ViewModelBase UserTripsTabViewModel
        {
            get => userTripsTabViewModel;
            set
            {
                userTripsTabViewModel = value;
                OnPropertyChanged();
            }
        }


        private ViewModelBase searchTabViewModel;
        public ViewModelBase SearchTabViewModel
        {
            get => searchTabViewModel;
            private set
            {
                searchTabViewModel = value;
                OnPropertyChanged();
            }
        }

        public void RouteSelected(int routeId, int departureId, int arrivalId, DateTime dateOfTrip)
        {
            vmMakeOrder.SelectRoute(routeId, departureId, arrivalId, dateOfTrip);
            SearchTabViewModel = vmMakeOrder;
        }

        public void OrderMade(Model.Order order, List<Model.Ticket> tickets)
        {
            //(window as MainWindow).DialogOk("Заказ добавлен");
            vmOrderResult.Order = order;
            vmOrderResult.Tickets = new ObservableCollection<Model.Ticket>(tickets);
            SearchTabViewModel = vmOrderResult;
        }

        public void NoFreeSeats()
        {
            SearchTabViewModel = vmSearchRoutes;
            vmSearchRoutes.ReloadDisplayedInfos();
        }

        public void OnRouteNotActive()
        {
            (window as MainWindow).DialogOk("Рейс отменён");
            SearchTabViewModel = vmSearchRoutes;
        }

        public void OrderSelected(Model.Order order, List<Model.Ticket> tickets)
        {
            vmOrderTickets.Order = order;
            vmOrderTickets.Tickets = new ObservableCollection<Model.Ticket>(tickets);
            UserTripsTabViewModel = vmOrderTickets;
        }

        public RelayCommand CmdBackToSearch { get; private set; }
        public RelayCommand CmdBackToUserOrders { get; private set; }

    }
	
}