using Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DTO = Interfaces.DTO;

namespace UserApp.ViewModel
{
    public class UserOrdersViewModel : ViewModelBase
    {

        private readonly IUserService userService;
        private readonly IRouteService routeService;

        public UserOrdersViewModel(Window window, IRouteService routeService, IUserService userService) 
        : base(window)
        {
            this.routeService = routeService;
            this.userService = userService;

            // Commands

            CmdSelectOrder = new RelayCommand(obj => 
            {
                var order = obj as Model.Order;

                var tickets = new List<Model.Ticket>();
                List<DTO.Ticket> ticketDTOs;

                try
                {
                    ticketDTOs = userService.GetOrderTickets(order.Id);
                }
                catch (Exception ex)
                {
                    (window as MainWindow).DialogOk(ex.Message);
                    return;
                }

                int i = 0;
                foreach (var t in ticketDTOs)
                {
                    tickets.Add(new Model.Ticket()
                    {
                        Id = t.Id.ToString("D12"),
                        Number = i,
                        FirstName = t.FirstName,
                        MiddleName = t.MiddleName,
                        LastName = t.LastName,
                        Gender = t.Gender,
                        Seat = t.Seat,
                        DOB = t.DOB,
                        Luggage = t.Luggage,
                        RouteId = orders.First(o => o.Id == order.Id).RouteId,
                        Price = t.Price
                    });
                    ++i;
                }

                OnOrderSelected(order, tickets);
            
            });

            CmdReloadOrders = new RelayCommand(obj =>
            {
                ReloadOrders();
            });

        }

        public void ReloadOrders()
        {

            orders.Clear();

            var orderDTOs = userService.GetOrders();

            foreach (var oDTO in orderDTOs)
            {

                var dep = routeService.GetDestinationById(oDTO.DeparturePoint);
                var arr = routeService.GetDestinationById(oDTO.ArrivalPoint);

                routeService.GetRoutePlatforms(oDTO.RouteId, oDTO.DeparturePoint, oDTO.ArrivalPoint, out int? deppl, out int? arrpl);
                routeService.GetRouteArrivalDepartureTime(oDTO.RouteId,
                                                          oDTO.DeparturePoint,
                                                          oDTO.ArrivalPoint,
                                                          out DateTime departureTime,
                                                          out DateTime arrivalTime);

                orders.Add(new Model.Order()
                {
                    Id = oDTO.Id,
                    RouteId = oDTO.RouteId,
                    DepartureCity = dep.Settlement,
                    DepartureInfo = FormatInfo(dep.Type, dep.TypeInfo),
                    DepartureDate = departureTime,
                    DeparturePlatform = deppl,
                    ArrivalCity = arr.Settlement,
                    ArrivalInfo = FormatInfo(arr.Type, arr.TypeInfo),
                    ArrivalDate = arrivalTime,
                    ArrivalPlatform = arrpl,
                    Date = oDTO.Date,
                    Price = userService.GetOrderTickets(oDTO.Id).Select(t => t.Price).Sum()
                });

            }

        }

        private string FormatInfo(string type, string typeInfo)
        {
            if (string.IsNullOrWhiteSpace(type))
                return "";

            string result = "";

            if (type == "АВ")
                result += "Автовокзал";
            else if (type == "АС")
                result += "Автостанция";

            if (!string.IsNullOrWhiteSpace(typeInfo))
                result += $" {typeInfo}";

            return result;

        }

        #region Bindings

        private ObservableCollection<Model.Order> orders = new ObservableCollection<Model.Order>();
        public ObservableCollection<Model.Order> Orders
        {
            get => orders;
            set
            {
                orders = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public RelayCommand CmdSelectOrder { get; private set; }
        public RelayCommand CmdReloadOrders { get; private set; }

        #endregion

        #region Events

        public delegate void OrderSelectedDelegate(Model.Order order, List<Model.Ticket> tickets);

        public event OrderSelectedDelegate OnOrderSelected;

        #endregion

    }

}