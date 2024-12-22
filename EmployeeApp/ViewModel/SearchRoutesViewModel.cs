using Interfaces.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using EmployeeApp.Model;
using DTO = Interfaces.DTO;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace EmployeeApp.ViewModel
{
    public class SearchRoutesViewModel : ViewModelBase
    {

        private readonly IRouteService routeService;

        public SearchRoutesViewModel(Window window, IRouteService routeService)
            : base(window)
        {
            this.routeService = routeService;
            routeInfoCards = new ObservableCollection<RouteInfoCard>();
            destinations = new ObservableCollection<DTO.Destination>();

            DateOfTrip = DateTime.Today;

            // Commands

            CmdSearchRoutes = new RelayCommand(obj =>
            {
                RouteInfoCards.Clear();

                displayedDepartureId = departureId;
                displayedArrivalId = arrivalId;

                var found = routeService.FindRoutes(departureId, arrivalId, dateOfTrip);

                if (found.Count == 0)
                    return;

                foreach (var info in found)
                {
                    var card = new RouteInfoCard();

                    List<DTO.Destination> ds;
                    decimal adultTicketPrice;
                    decimal underageTicketPrice;
                    int travelHours;
                    byte travelMinutes;
                    TimeSpan departureTime;
                    TimeSpan arrivalTime;

                    try
                    {

                        ds = routeService.GetFullRouteInfo(
                             info.Id,
                             null,
                             null,
                             out adultTicketPrice,
                             out underageTicketPrice,
                             out travelHours,
                             out travelMinutes,
                             out departureTime,
                             out arrivalTime);

                    }
                    catch (Exception ex)
                    {

                        (window as MainWindow).DialogOk(ex.Message);
                        return;

                    }

                    card.Id = info.Id;

                    card.DeparturePoint = ds.First(e => e.Id == departureId).Name;
                    card.DepartureTime = string.Format("{0:hh\\:mm}", departureTime);
                    card.DepartureDate = "";

                    string hh = "";

                    if (travelHours == 0)
                        hh = "0";
                    else
                        hh = travelHours.ToString();

                    string mm = "";
                    if (travelMinutes == 0)
                        mm = "00";
                    else
                        mm = travelMinutes.ToString();

                    card.TravelTime = $"{hh}ч {mm}м";

                    card.ArrivalPoint = ds.First(e => e.Id == arrivalId).Name;
                    card.ArrivalTime = string.Format("{0:hh\\:mm}", arrivalTime);
                    card.ArrivalDate = "";
                    card.BusCompany = routeService.GetBusCompanyById(routeService.GetTransportById(info.TransportId).BusCompanyId).Name;
                    card.Schedule = string.Join(", ", routeService.ScheduleMaskToWeekdays(info.Schedule));
                    card.AdultPrice = adultTicketPrice.ToString("N2") + "₽";
                    card.UnderagePrice = underageTicketPrice.ToString("N2") + "₽";
                    card.ScheduleMask = info.Schedule;

                    RouteInfoCards.Add(card);
                }

            });

            CmdSelectRoute = new RelayCommand(obj => 
            {
                var info = obj as Model.RouteInfoCard;
                OnRouteSelected(info.Id, displayedDepartureId, displayedArrivalId, DateOfTrip);
            });

        }

        public void ReloadDestinations()
        {
            destinations.Clear();
            var reloaded = routeService.GetAllDestinations();
            foreach (var dest in reloaded)
            {
                destinations.Add(dest);
            }
        }

        public void ReloadDisplayedInfos()
        {
            CmdSearchRoutes.Execute(null);
        }

        #region Bindings

        private ObservableCollection<RouteInfoCard> routeInfoCards;
        public ObservableCollection<RouteInfoCard> RouteInfoCards
        {
            get => routeInfoCards;
            set
            {
                routeInfoCards = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<DTO.Destination> destinations;
        public ObservableCollection<DTO.Destination> Destinations 
        {
            get => destinations;
            set
            {
                destinations = value;
                OnPropertyChanged();
            }
        }

        private DateTime dateOfTrip;
        public DateTime DateOfTrip
        {
            get => dateOfTrip;
            set
            {
                dateOfTrip = value;
                OnPropertyChanged("DateOfTrip");
            }
        }

        private int displayedDepartureId;
        private int departureId;
        public int DepartureId
        {
            get => departureId;
            set
            {
                departureId = value;
                OnPropertyChanged();
            }
        }

        private int displayedArrivalId;
        private int arrivalId;
        public int ArrivalId
        {
            get => arrivalId;
            set
            {
                arrivalId = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public RelayCommand CmdSearchRoutes { get; private set; }
        public RelayCommand CmdSelectRoute { get; private set; }

        #endregion

        #region Events

        public delegate void RouteSelectedDelegate(int routeId, int departureId, int arrivalId, DateTime dateOfTrip);

        public event RouteSelectedDelegate OnRouteSelected;

        #endregion

    }

}