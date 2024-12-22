using Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UserApp.Model;
using DTO = Interfaces.DTO;

namespace UserApp.ViewModel
{
    public class ReportViewModel : ViewModelBase
    {

        private readonly IUserService userService;
        private readonly IUserService routeService;

        public enum SearchTypes
        {
            ThisMonth,
            ThisYear,
            Overall,
            Custom
        }

        public enum ReportStatus
        {
            NotCreated,
            Created,
            CreatedEmpty
        }

        public ReportViewModel(Window window, IUserService userService, IRouteService routeService) 
        : base(window)
        {
            this.userService = userService;

            // Commands

            CmdGetReport = new RelayCommand(obj =>
            {
                DateTime? start = null;
                DateTime? end = null;

                if (searchType.Value == SearchTypes.ThisMonth)
                {
                    start = new DateTime(DateTime.Now.Year,
                                         DateTime.Now.Month,
                                         1);

                    end = new DateTime(DateTime.Now.Year,
                                       DateTime.Now.Month,
                                       DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                }
                else if (searchType.Value == SearchTypes.ThisYear)
                {
                    start = new DateTime(DateTime.Now.Year, 1, 1);
                    end = new DateTime(DateTime.Now.Year, 12, 31);
                }
                else if (searchType.Value == SearchTypes.Custom)
                {
                    start = startDate.Value;
                    end = endDate.Value;
                }

                Dictionary<DTO.Order, List<DTO.Ticket>> report;

                try {
                    report = userService.GetPersonalReport(start, end);
                } 
                catch (Exception ex)
                {
                    (window as MainWindow).DialogOk(ex.Message);
                    return;
                }

                ReportData = null;
                ReportPositions.Clear();

                if (report.Count == 0)
                    return;

                ReportData = new ReportData();

                var popularity = new Dictionary<int, int>();

                foreach (var position in report)
                {

                    var order = position.Key;
                    var tickets = position.Value;

                    var routeInfo = routeService.GetRouteInfo(order.RouteId);

                    if (popularity.ContainsKey(routeInfo.Id))
                        popularity[routeInfo.Id]++;
                    else
                        popularity.Add(routeInfo.Id, 1);

                    List<DTO.Destination> ds1;
                    decimal adultTicketPrice1;
                    decimal underageTicketPrice1;
                    int travelHours1;
                    byte travelMinutes1;
                    TimeSpan departureTime1;
                    TimeSpan arrivalTime1;

                    try
                    {
                        ds1 = routeService.GetFullRouteInfo(
                              routeInfo.Id,
                              order.DeparturePoint,
                              order.ArrivalPoint,
                              out adultTicketPrice1,
                              out underageTicketPrice1,
                              out travelHours1,
                              out travelMinutes1,
                              out departureTime1,
                              out arrivalTime1);
                    }
                    catch (Exception ex)
                    {
                        (window as MainWindow).DialogOk(ex.Message);
                        return;
                    }

                    var times = routeService.GetRouteTimes(order.RouteId);
                    int departureIndex = ds1.IndexOf(ds1.Find(d => d.Id == order.DeparturePoint));
                    int arrivalIndex = ds1.IndexOf(ds1.Find(d => d.Id == order.ArrivalPoint));

                    var pos = new Model.ReportPositionData()
                    {
                        OrderId = order.Id,
                        AdultPrice = adultTicketPrice1,
                        AdultCount = tickets.Where(t => t.Price == adultTicketPrice1).Count(),
                        UnderagePrice = underageTicketPrice1,
                        UnderageCount = tickets.Where(t => t.Price == underageTicketPrice1).Count(),
                        RouteInfoId = routeInfo.Id,
                        RouteId = order.RouteId,
                        DepartureCity = ds1[departureIndex].SettlementName,
                        DepartureDate = times[departureIndex],
                        ArrivalCity = ds1[arrivalIndex].SettlementName,
                        ArrivalDate = times[arrivalIndex],
                        TravelTime = times[arrivalIndex] - times[departureIndex]
                    };

                    ReportPositions.Add(pos);
                    ReportData.OverallPrice += pos.TotalPrice;
                    ReportData.OverallTravelTime += pos.TravelTime;

                }

                int max = 0;
                List<int> candidates = new List<int>();

                foreach (var p in popularity)
                {
                    if (p.Value > max)
                    {
                        max = p.Value;
                        candidates.Clear();
                        candidates.Add(p.Key);
                    }
                    else if (p.Value == max)
                    {
                        candidates.Add(p.Key);
                    }  
                }

                var random = new Random();
                var mostPopularRouteId = candidates[random.Next(candidates.Count)];
                var mostPopularRouteInfo = routeService.GetRouteInfoById(mostPopularRouteId);
                var mostPopularTransport = routeService.GetTransportById(mostPopularRouteInfo.TransportId);

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
                         mostPopularRouteId,
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

                var mostPopularUserOrders = ReportPositions.Where(p => p.RouteInfoId == mostPopularRouteId);

                ReportData.MostUsedRouteInfoId = mostPopularRouteId;
                ReportData.MostUsedOrderCount = mostPopularUserOrders.Count();
                ReportData.MostUsedSpent = mostPopularUserOrders.Sum(o => o.TotalPrice);
                ReportData.MostUsedDeparture = ds[0].SettlementName;
                ReportData.MostUsedArrival = ds.Last().SettlementName;
                ReportData.MostUsedCompany = routeService.GetBusCompanyById(mostPopularTransport.BusCompanyId).Name;

                ReportCreated = true;

            },
            obj =>
            {
                if (!searchType.HasValue)
                    return false;

                if (searchType.Value == SearchTypes.ThisMonth || 
                    searchType.Value == SearchTypes.ThisYear ||
                    searchType.Value == SearchTypes.Overall)
                    return true;

                if (searchType.Value == SearchTypes.Custom &&
                    (startDate == null ||
                    endDate == null ||
                    (startDate > endDate)))
                    return false;

                return true;
            });

        }

        #region Bindings

        private SearchTypes? searchType;
        public SearchTypes? SearchType
        {
            get => searchType;
            set
            {
                searchType = value;
                OnPropertyChanged();
            }
        }

        private DateTime? startDate;
        public DateTime? StartDate
        {
            get => startDate;
            set
            {
                startDate = value;
                OnPropertyChanged();
            }
        }

        private DateTime? endDate;
        public DateTime? EndDate
        {
            get => endDate;
            set
            {
                endDate = value;
                OnPropertyChanged();
            }
        }

        private bool reportCreated = false;
        public bool ReportCreated
        {
            get => reportCreated;
            set
            {
                reportCreated = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Model.ReportPositionData> reportPositions = new ObservableCollection<Model.ReportPositionData>();
        public ObservableCollection<Model.ReportPositionData> ReportPositions
        {
            get => reportPositions;
            set
            {
                reportPositions = value;
                OnPropertyChanged();
            }
        }

        private Model.ReportData reportData;
        public Model.ReportData ReportData
        {
            get => reportData;
            set
            {
                reportData = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public RelayCommand CmdGetReport { get; private set; }

        #endregion

    }
}
