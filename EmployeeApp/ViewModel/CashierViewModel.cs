using Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EmployeeApp.ViewModel
{
    public class CashierViewModel : ViewModelBase
    {

        private readonly IRouteService routeService;
        private readonly IUserService userService;

        public CashierViewModel(Window window, IRouteService routeService, IUserService userService)
        : base(window)
        {
            this.routeService = routeService;
            this.userService = userService;

            vmSearchRoutes = new SearchRoutesViewModel(window, routeService);
            vmMakeOrder = new MakeOrderViewModel(window, routeService, userService);
            vmOrderResult = new OrderResultViewModel(window);

            vmSearchRoutes.OnRouteSelected += vmMakeOrder.SelectRoute;
            vmSearchRoutes.OnRouteSelected += RouteSelected;

            vmMakeOrder.OnMakeOrder += OrderMade;
            vmMakeOrder.OnNoFreeSeats += NoFreeSeats;
            vmMakeOrder.OnRouteNotActive += OnRouteNotActive;

            SearchTabViewModel = vmSearchRoutes;
            vmSearchRoutes.ReloadDestinations();

            CmdBackToSearch = new RelayCommand(obj =>
            {
                SearchTabViewModel = vmSearchRoutes;
            });

        }

        #region Bindings

        public SearchRoutesViewModel vmSearchRoutes { get; private set; }
        public MakeOrderViewModel vmMakeOrder { get; private set; }
        public OrderResultViewModel vmOrderResult { get; private set; }

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

        #endregion

        #region Commands

        public RelayCommand CmdBackToSearch { get; private set; }

        #endregion

        #region Events

        public void RouteSelected(int routeId, int departureId, int arrivalId, DateTime dateOfTrip)
        {
            SearchTabViewModel = vmMakeOrder;
        }

        public void OrderMade(Model.Order order, List<Model.Ticket> tickets)
        {
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

        #endregion

    }
}
