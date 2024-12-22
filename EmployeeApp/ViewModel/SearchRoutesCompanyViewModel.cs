using Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DTO = Interfaces.DTO;
using Model = EmployeeApp.Model;

namespace EmployeeApp.ViewModel
{
    public class SearchRoutesCompanyViewModel : ViewModelBase
    {

        private readonly IRouteService routeService;

        public SearchRoutesCompanyViewModel(Window window, IRouteService routeService)
        : base(window)
        {
            this.routeService = routeService;
            routeInfoCards = new ObservableCollection<Model.RouteInfoCard>();
            busCompanies = new ObservableCollection<DTO.BusCompany>();

            // Commands

            CmdSearchRoutes = new RelayCommand(obj =>
            {
                RouteInfoCards.Clear();

                var found = routeService.FindRoutesByCompany(busCompanyId);

                if (found.Count == 0)
                    return;

                foreach (var info in found)
                {
                    var card = new Model.RouteInfoCard();

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

                    card.DeparturePoint = ds.First().Name;
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

                    card.ArrivalPoint = ds.Last().Name;
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
                OnRouteSelected(info);
            });

        }

        public void ReloadBusCompanies()
        {
            busCompanies.Clear();
            var reloaded = routeService.GetAllBusCompanies();
            foreach (var dest in reloaded)
            {
                busCompanies.Add(dest);
            }
        }

        public void ReloadDisplayedInfos()
        {
            CmdSearchRoutes.Execute(null);
        }

        #region Bindings

        private ObservableCollection<Model.RouteInfoCard> routeInfoCards;
        public ObservableCollection<Model.RouteInfoCard> RouteInfoCards
        {
            get => routeInfoCards;
            set
            {
                routeInfoCards = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<DTO.BusCompany> busCompanies;
        public ObservableCollection<DTO.BusCompany> BusCompanies
        {
            get => busCompanies;
            set
            {
                busCompanies = value;
                OnPropertyChanged();
            }
        }

        private int busCompanyId;
        public int BusCompanyId
        {
            get => busCompanyId;
            set
            {
                busCompanyId = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public RelayCommand CmdSearchRoutes { get; private set; }
        public RelayCommand CmdSelectRoute { get; private set; }

        #endregion

        #region Events

        public delegate void RouteSelectedDelegate(Model.RouteInfoCard card);

        public event RouteSelectedDelegate OnRouteSelected;

        #endregion

    }
}
