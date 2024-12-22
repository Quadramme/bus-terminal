using Interfaces.Services;
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
    public class CreateTripsViewModel : ViewModelBase
    {
        private readonly IRouteService routeService;

        public CreateTripsViewModel(Window window, IRouteService routeService) 
        : base(window)
        {
            this.routeService = routeService;

            // Commands

            CmdCreateTrip = new RelayCommand(obj =>
            {
                if (!newTripDate.HasValue)
                {
                    (window as MainWindow).DialogOk("Выберите дату поездки");
                    return;
                }

                if ((routeService.DateToScheduleMask(newTripDate.Value) & routeInfo.ScheduleMask) == 0)
                {
                    (window as MainWindow).DialogOk("Дата не подходит для расписания маршрута");
                    return;
                }

                var trip = routeService.GetRouteInfoRoutes(routeInfo.Id)
                                       .FirstOrDefault(t => t.Date == newTripDate.Value);

                if (trip != null)
                {
                    (window as MainWindow).DialogOk("Такая поездка уже существует");
                    return;
                }

                try {

                    routeService.CreateRoute(routeInfo.Id, newTripDate.Value);

                } 
                catch (Exception ex)
                {

                    (window as MainWindow).DialogOk(ex.Message);
                    return;

                }

                LoadTrips(routeInfo);

            });

        }

        public void LoadTrips(Model.RouteInfoCard routeInfo)
        {
            RouteInfo = routeInfo;

            var routes = routeService.GetRouteInfoRoutes(routeInfo.Id)
                                     .Where(r => r.RouteStatusId == 1)
                                     .ToList();

            routeService.GetRouteInfoPlatforms(routeInfo.Id, out int? departurePlatform, out int? arrivalPlatform);

            trips.Clear();
            Trips = new ObservableCollection<Model.Trip>(routes.Select(r => new Model.Trip()
            {
                Date = r.Date,
                Platform = departurePlatform,
                AvailableSeats = routeService.GetFreeRouteSeats(r.Id).Count,
                AvailableLuggageSpace = r.LuggageSpace
            }));
        }

        #region Bindings

        private ObservableCollection<Model.Trip> trips = new ObservableCollection<Model.Trip>();
        public ObservableCollection<Model.Trip> Trips
        {
            get => trips;
            set
            {
                trips = value;
                OnPropertyChanged();
            }
        }

        private DateTime? newTripDate;
        public DateTime? NewTripDate
        {
            get => newTripDate;
            set
            {
                newTripDate = value;
                OnPropertyChanged();
            }
        }

        private Model.RouteInfoCard routeInfo;
        public Model.RouteInfoCard RouteInfo
        {
            get => routeInfo;
            set
            {
                routeInfo = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public RelayCommand CmdCreateTrip { get; private set; }

        #endregion

        #region Events



        #endregion

    }

}