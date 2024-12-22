using Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EmployeeApp.ViewModel
{
    public class OperatorViewModel : ViewModelBase
    {

        private readonly IRouteService routeService;
        private readonly IUserService userService;

        public OperatorViewModel(Window window, IRouteService routeService, IUserService userService) 
        : base(window)
        {
            this.routeService = routeService;
            this.userService = userService;

            vmCreateRoute = new CreateRouteViewModel(window, routeService, userService);
            vmSearchRoutesCompany = new SearchRoutesCompanyViewModel(window, routeService);
            vmCreateTrips = new CreateTripsViewModel(window, routeService);

            vmSearchRoutesCompany.OnRouteSelected += OnRouteSelected;
            vmSearchRoutesCompany.ReloadBusCompanies();

            TripsTabViewModel = vmSearchRoutesCompany;

            CmdBackToSearch = new RelayCommand(obj =>
            {
                TripsTabViewModel = vmSearchRoutesCompany;
            });

        }

        #region Bindings

        public CreateRouteViewModel vmCreateRoute { get; private set; }

        private ViewModelBase tripsTabViewModel;
        public ViewModelBase TripsTabViewModel 
        {
            get => tripsTabViewModel;
            private set
            {
                tripsTabViewModel = value;
                OnPropertyChanged();
            }
        }

        public SearchRoutesCompanyViewModel vmSearchRoutesCompany { get; private set; }

        public CreateTripsViewModel vmCreateTrips { get; private set; }

        #endregion

        #region Commands

        public RelayCommand CmdBackToSearch { get; private set; }

        #endregion

        #region Events

        public void OnRouteSelected(Model.RouteInfoCard card)
        {
            vmCreateTrips.LoadTrips(card);
            TripsTabViewModel = vmCreateTrips;
        }

        #endregion

    }
}
