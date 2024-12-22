using Interfaces.DTO;
using Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using EmployeeApp.Model;
using DTO = Interfaces.DTO;
using Model = EmployeeApp.Model;

namespace EmployeeApp.ViewModel
{
    public class MakeOrderViewModel : ViewModelBase
    {

        private readonly IRouteService routeService;
        private readonly IUserService userService;
        private int routeId;
        private int routeInfoId;
        private int departureId;
        private int arrivalId;

        public MakeOrderViewModel(Window window, IRouteService routeService, IUserService userService)
            : base(window)
        {
            this.routeService = routeService;
            this.userService = userService;

            // Commands

            CmdAddTicket = new RelayCommand(obj => 
            {

                var newTicket = new Model.Ticket()
                {
                    Number = tickets.Count + 1,
                    RouteId = routeId
                };

                newTicket.PropertyChanged += OnTicketSeatChanged;
                tickets.Add(newTicket);

                OnPropertyChanged("AdultTicketCount");
                OnPropertyChanged("UnderageTicketCount");
                OnPropertyChanged("TotalOrderPrice");

                RecountLuggageLimits();

            });

            CmdRemoveTicket = new RelayCommand(obj =>
            {
                int index = (int)obj - 1;
                tickets.RemoveAt(index);

                for (int i = 0; i < tickets.Count; ++i)
                {
                    tickets[i].Number = i + 1;
                }

                OnPropertyChanged("AdultTicketCount");
                OnPropertyChanged("UnderageTicketCount");
                OnPropertyChanged("TotalOrderPrice");
                OnPropertyChanged("NotOccupiedSeatsCount");

                RecountLuggageSpaceLeft();
                RecountLuggageLimits();

            });

            CmdMakeOrder = new RelayCommand(obj =>
            {
                var orderDTO = new DTO.Order()
                {
                    RouteId = routeId,
                    DeparturePoint = departureId,
                    ArrivalPoint = arrivalId,
                    PhoneNumber = PhoneNumber,
                    Email = Email,
                };

                var ticketDTOs = new List<DTO.Ticket>();

                foreach (var t in tickets)
                {
                    ticketDTOs.Add(new DTO.Ticket()
                    {
                        FirstName = t.FirstName,
                        MiddleName = t.MiddleName,
                        LastName = t.LastName,
                        Seat = t.Seat.Value,
                        DOB = t.DOB.Value,
                        Luggage = t.Luggage,
                        Price = t.DOB.Value.AddYears(18) > DateTime.Now ? UnderageTicketPrice : adultTicketPrice
                    });
                }

                var success = userService.MakeOrder(orderDTO, ticketDTOs);

                switch (success)
                {
                    case MakeOrderStatus.RequiredSeatsOccupied:
                        {
                            var free = routeService.GetFreeRouteSeats(routeId);
                            var toDelete = freeSeats.Where(s => !free.Contains(s)).ToList();

                            foreach (var del in toDelete)
                            {
                                freeSeats.Remove(del);
                            }

                            (window as MainWindow).DialogOk("Некоторые места были заняты другими пассажирами. Пожалуйста, выберите другие");

                            OnPropertyChanged("NotOccupiedSeatsCount");

                            return;
                        }

                    case MakeOrderStatus.NoSeatsAvailable:
                        {

                            (window as MainWindow).DialogOk("На данном рейсе не осталось свободных мест");
                            OnNoFreeSeats();
                            return;
                        }

                    case MakeOrderStatus.NotEnoughLuggageSpace:
                        {
                            FreeLuggageSpace = routeService.GetFreeLuggageSpace(routeId);

                            int occupied = 0;

                            foreach (var t in tickets)
                                occupied += t.Luggage;

                            int lower = occupied - freeLuggageSpace;

                            foreach (var t in tickets)
                            {
                                if (t.Luggage == 0)
                                    continue;

                                if (lower > t.Luggage)
                                {
                                    lower -= t.Luggage;
                                    t.Luggage = 0;
                                }
                                else
                                {
                                    t.Luggage -= lower;
                                    break;
                                }

                            }

                            RecountLuggageSpaceLeft();
                            RecountLuggageLimits();

                            (window as MainWindow).DialogOk("К сожалению, места для вашего багажа недостаточно");

                            return;
                        }
                }

                for (int i = 0; i < tickets.Count; ++i)
                {
                    tickets[i].Id = ticketDTOs[i].Id.ToString("D12");
                    tickets[i].Price = ticketDTOs[i].Price;
                }

                var dep = DepartureDestination;
                var arr = ArrivalDestination;

                string departureInfo = "", arrivalInfo = "";

                if (dep.Type == "АВ")
                    departureInfo += "Автовокзал";
                else if (dep.Type == "АС")
                    departureInfo += "Автостанция";
                else
                    departureInfo = null;

                if (arr.Type == "АВ")
                    arrivalInfo += "Автовокзал";
                else if (arr.Type == "АС")
                    arrivalInfo += "Автостанция";
                else
                    arrivalInfo = null;

                routeService.GetRoutePlatforms(routeId, departureId, arrivalId, out int? deppl, out int? arrpl);

                var o = new Model.Order()
                {
                    Id = orderDTO.Id,
                    RouteId = orderDTO.RouteId,
                    DepartureCity = dep.Settlement,
                    DepartureInfo = departureInfo,
                    DepartureDate = dep.ArrivalTime,
                    DeparturePlatform = deppl,
                    ArrivalCity = arr.Settlement,
                    ArrivalInfo = arrivalInfo,
                    ArrivalDate = arr.ArrivalTime,
                    ArrivalPlatform = arrpl,
                    Date = orderDTO.Date
                };

                OnMakeOrder(o, tickets.ToList());

            },
            obj =>
            {

                if (Email == null || PhoneNumber == null)
                    return false;

                var seats = new List<int>();

                foreach (var t in tickets)
                {
                    if (t.LastName == null ||
                        t.FirstName == null ||
                        t.DOB == null ||
                        t.Seat == null ||
                        t.Gender == null)
                        return false;

                    if (seats.Contains(t.Seat.Value))
                        return false;

                    seats.Add(t.Seat.Value);

                }

                return true;

            });

        }

        public void SelectRoute(int routeInfoId, int departureId, int arrivalId, DateTime dateOfTrip)
        {

            int routeId;
            int status;

            try
            {
                routeService.GetRouteStatus(routeInfoId, dateOfTrip, out routeId, out status);
            }
            catch (Exception ex)
            {
                (window as MainWindow).DialogOk(ex.Message);
                return;
            }

            if (status != 1)
            {
                OnRouteNotActive();
                return;
            }

            freeSeats.Clear();
            var free = routeService.GetFreeRouteSeats(routeId);
            foreach (var s in free)
            {
                freeSeats.Add(s);
            }

            if (freeSeats.Count == 0)
            {
                OnNoFreeSeats();
                return;
            }

            FreeLuggageSpace = routeService.GetFreeLuggageSpace(routeId);
            RecountLuggageSpaceLeft();

            var passenger = userService.CurrentUser as DTO.Passenger;

            if (passenger != null)
            {
                PhoneNumber = passenger.PhoneNumber;
                Email = passenger.Email;
            }

            this.routeInfoId = routeInfoId;
            this.routeId = routeId;
            this.departureId = departureId;
            this.arrivalId = arrivalId;
            DateOfTrip = dateOfTrip;

            var info = routeService.GetRouteInfoById(routeInfoId);

            var transport = routeService.GetTransportById(info.TransportId);

            TransportType = routeService.GetTransportTypeById(transport.TransportTypeId).Name;
            TotalPassengerSeats = transport.MaxSeats;
            TotalLuggageSpace = transport.MaxLuggage;

            List<DTO.Destination> ds;
            int travelHours;
            byte travelMinutes;
            TimeSpan departureTime;
            TimeSpan arrivalTime;

            try
            {

                ds = routeService.GetFullRouteInfo(
                     routeInfoId,
                     departureId,
                     arrivalId,
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

            var times = routeService.GetRouteTimes(routeId);
            travelTime = $"{(travelHours == 0 ? "0" : travelHours.ToString())}ч {(travelMinutes == 0 ? "00" : travelMinutes.ToString())}м";

            var modelDs = new ObservableCollection<Model.Destination>();

            int iStart = ds.IndexOf(ds.First(d => d.Id == departureId));
            int iEnd = ds.IndexOf(ds.First(d => d.Id == arrivalId));

            for (int i = 0; i < ds.Count; ++i)
            {

                var d = ds[i];

                modelDs.Add(new Model.Destination()
                {
                    Id = d.Id,
                    Region = routeService.GetRegionById(d.RegionId).Name,
                    Type = d.Type,
                    TypeInfo = d.TypeInfo,
                    Settlement = d.Settlement,
                    Street = d.Street,
                    House = d.House,
                    ArrivalTime = times[i],
                    IsVisited = i >= iStart && i <= iEnd
                });

            }

            if (Destinations != null)
                Destinations.Clear();

            Destinations = modelDs;

            if (Tickets != null)
                Tickets.Clear();

            Tickets = new ObservableCollection<Model.Ticket>();

            CmdAddTicket.Execute(null);

        }

        private void RecountLuggageSpaceLeft()
        {
            int occupied = 0;

            foreach (var t in tickets)
            {
                occupied += t.Luggage;
            }

            LuggageSpaceLeft = freeLuggageSpace - occupied;

        }

        private void RecountLuggageLimits()
        {
            
            foreach (var t in tickets)
            {
                int overflow = t.Luggage + luggageSpaceLeft;

                if (overflow >= 3)
                    t.MaxLuggage = 3;
                else
                    t.MaxLuggage = overflow;
                
            }

        }

        #region Bindings

        private string phoneNumber;
        public string PhoneNumber
        {
            get => phoneNumber;
            set
            {
                phoneNumber = value;
                OnPropertyChanged();
            }
        }

        private string email;
        public string Email
        {
            get => email;
            set
            {
                email = value;
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

        private ObservableCollection<int> freeSeats = new ObservableCollection<int>();
        public ObservableCollection<int> FreeSeats
        {
            get => freeSeats;
            set
            {
                freeSeats = value;
                OnPropertyChanged();
            }
        }

        public int NotOccupiedSeatsCount
        {
            get
            {
                var occupied = new List<int>();

                foreach (var t in tickets)
                {
                    if (t.Seat.HasValue)
                        occupied.Add(t.Seat.Value);
                }

                return freeSeats
                            .Where(s => !occupied.Contains(s))
                            .Count();

            }
        }

        private ObservableCollection<Model.Destination> destinations = new ObservableCollection<Model.Destination>();
        public ObservableCollection<Model.Destination> Destinations
        {
            get => destinations;
            set
            {
                destinations = value;
                OnPropertyChanged();
                OnPropertyChanged("DepartureDestination");
                OnPropertyChanged("ArrivalDestination");
            }
        }

        public Model.Destination DepartureDestination
        {
            get => destinations.First(e => e.Id == departureId);
        }

        public Model.Destination ArrivalDestination
        {
            get => destinations.First(e => e.Id == arrivalId);
        }

        private DateTime dateOfTrip;
        public DateTime DateOfTrip
        {
            get => dateOfTrip;
            set
            {
                dateOfTrip = value;
                OnPropertyChanged();
            }
        }

        private string travelTime;
        public string TravelTime
        {
            get => travelTime;
            private set
            {
                travelTime = value;
                OnPropertyChanged();
            }
        }

        private string transportType;
        public string TransportType
        {
            get => transportType;
            private set
            {
                transportType = value;
                OnPropertyChanged();
            }
        }

        private int totalPassengerSeats;
        public int TotalPassengerSeats
        {
            get => totalPassengerSeats;
            private set
            {
                totalPassengerSeats = value;
                OnPropertyChanged();
            }
        }

        private int totalLuggageSpace;
        public int TotalLuggageSpace
        {
            get => totalLuggageSpace;
            private set
            {
                totalLuggageSpace = value;
                OnPropertyChanged();
            }
        }

        private int freeLuggageSpace;
        public int FreeLuggageSpace
        {
            get => freeLuggageSpace;
            set
            {
                freeLuggageSpace = value;
                OnPropertyChanged();
            }
        }

        private int luggageSpaceLeft;
        public int LuggageSpaceLeft
        {
            get => luggageSpaceLeft;
            set
            {
                luggageSpaceLeft = value;
                OnPropertyChanged();
            }
        }

        private decimal adultTicketPrice;
        public decimal AdultTicketPrice
        {
            get => adultTicketPrice;
            set
            {
                adultTicketPrice = value;
                OnPropertyChanged();
                OnPropertyChanged("TotalPrice");
            }
        }

        private decimal underageTicketPrice;
        public decimal UnderageTicketPrice
        {
            get => underageTicketPrice;
            set
            {
                underageTicketPrice = value;
                OnPropertyChanged();
                OnPropertyChanged("TotalPrice");
            }
        }

        public decimal TotalOrderPrice
        {
            get => adultTicketPrice * AdultTicketCount + underageTicketPrice * UnderageTicketCount;
        }

        public int AdultTicketCount
        {
            get => tickets
                        .Where(t => t.DOB == null || (t.DOB != null && t.DOB.Value.AddYears(18) <= DateTime.Now))
                        .Count();
        }

        public int UnderageTicketCount
        {
            get => tickets
                        .Where(t => t.DOB != null && t.DOB.Value.AddYears(18) > DateTime.Now)
                        .Count();
        }

        #endregion

        #region Commands

        public RelayCommand CmdAddTicket { get; private set; }
        public RelayCommand CmdRemoveTicket { get; private set; }
        public RelayCommand CmdMakeOrder { get; private set; }

        #endregion

        #region Events

        public void OnTicketSeatChanged(object sender, PropertyChangedEventArgs e)
        {

            switch (e.PropertyName)
            {
                case "Seat":
                    OnPropertyChanged("NotOccupiedSeatsCount");
                    break;

                case "DOB":
                    OnPropertyChanged("AdultTicketCount");
                    OnPropertyChanged("UnderageTicketCount");
                    OnPropertyChanged("TotalOrderPrice");
                    break;

                case "Luggage":
                    RecountLuggageSpaceLeft();
                    RecountLuggageLimits();
                    break;

            }

        }

        // В MainViewModel

        public delegate void MakeOrderDelegate(Model.Order order, List<Model.Ticket> tickets);

        public event MakeOrderDelegate OnMakeOrder;

        public event Action OnNoFreeSeats;

        public delegate void RouteNotActiveDelegate();

        public event RouteNotActiveDelegate OnRouteNotActive;

        #endregion

    }
}