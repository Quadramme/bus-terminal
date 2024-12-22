using Interfaces.Repository;
using Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using DTO = Interfaces.DTO;
using DM = DomainModel;

namespace BLL.Services
{

    public class UserService : IUserService
    {

        public UserService(IDbRepository repo, Type userType) 
        {
            dbr = repo;

            if (!userType.IsSubclassOf(typeof(DTO.User)))
                throw new Exception("UserType must be derived from DTO.User");

            this.userType = userType;
        }

        public bool CanConnect()
        {
            return dbr.CanConnect();
        }

        public bool IsLoggedIn { get => (currentUser != null); }

        public DTO.User CurrentUser { get => currentUser; }

        LoginStatus IUserService.Login(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException();

            var user = dbr.Filters.GetUser(login, password, out LoginStatus status);

            if (status == LoginStatus.Success) 
            {

                if (userType.Equals(typeof(DTO.Passenger)))
                {
                    var passenger = user as DM.Passenger;

                    if (passenger == null)
                        return LoginStatus.UserDoesNotExist;

                    var passengerDTO = new DTO.Passenger()
                    {
                        Id = passenger.Id,
                        FirstName = passenger.FirstName,
                        MiddleName = passenger.MiddleName,
                        LastName = passenger.LastName,
                        RegistrationDate = passenger.RegistrationDate,
                        PhoneNumber = passenger.PhoneNumber,
                        Email = passenger.Email
                    };

                    currentUser = passengerDTO;
                }
                else if (userType.Equals(typeof(DTO.Employee)))
                {
                    var employee = user as DM.Employee;

                    if (employee == null)
                        return LoginStatus.UserDoesNotExist;

                    var employeeDTO = new DTO.Employee()
                    {
                        Id = employee.Id,
                        FirstName = employee.FirstName,
                        MiddleName = employee.MiddleName,
                        LastName = employee.LastName,
                        RegistrationDate = employee.RegistrationDate,
                        EmployeeType = employee.EmployeeTypeId
                    };

                    currentUser = employeeDTO;
                }

            }

            return status;

        }

        public void Logout()
        {
            currentUser = null;
        }

        public RegisterStatus Register(DTO.Passenger passenger)
        {

            if (userType != typeof(DTO.Passenger))
                throw new Exception("Service isn't set up for passengers");

            if (string.IsNullOrWhiteSpace(passenger.Login) ||
                string.IsNullOrWhiteSpace(passenger.Password) ||
                string.IsNullOrWhiteSpace(passenger.FirstName) ||
                string.IsNullOrWhiteSpace(passenger.LastName))
                throw new ArgumentNullException();

            if (dbr.Filters.CheckLogin(passenger.Login))
                return RegisterStatus.UserAlreadyExists;

            dbr.Passengers.Create(new DM.Passenger()
            {
                Login = passenger.Login,
                Password = passenger.Password,
                FirstName = passenger.FirstName,
                MiddleName = passenger.MiddleName,
                LastName = passenger.LastName,
                RegistrationDate = DateTime.Now,
                PhoneNumber = passenger.PhoneNumber,
                Email = passenger.Email
            });

            dbr.Save();

            return RegisterStatus.Success;
        }

        public MakeOrderStatus MakeOrder(DTO.Order order, List<DTO.Ticket> tickets)
        {

            // Проверка параметров

            if (order == null || tickets == null)
                throw new ArgumentNullException();

            if (tickets.Count == 0)
                throw new Exception("At least one ticket must be provided");

            var route = dbr.Routes.Get(order.RouteId);

            if (route == null)
                throw new Exception("Route does not exist");

            if (dbr.Destinations.Get(order.DeparturePoint) == null)
                throw new Exception("Departure destination does not exist");

            if (dbr.Destinations.Get(order.ArrivalPoint) == null)
                throw new Exception("Arrival destination does not exist");

            if (string.IsNullOrWhiteSpace(order.PhoneNumber))
                throw new Exception("Phone number is required");

            if(string.IsNullOrWhiteSpace(order.Email))
                throw new Exception("Email is required");

            // Если покупает пассажир

            if (currentUser != null && 
                userType == typeof(DTO.Passenger))
            {
                order.PassengerId = currentUser.Id;
            } 
            else
            {
                order.PassengerId = null;
            }

            // Расчёт цен
            decimal adultPrice = 0m;
            decimal underagePrice = 0m;

            var rels = dbr.Filters.GetRouteInfoRels(route.RouteInfoId);
            var dep = rels.FirstOrDefault(r => r.DestinationId == order.DeparturePoint);
            var arr = rels.FirstOrDefault(r => r.DestinationId == order.ArrivalPoint);

            if (dep == null)
                throw new Exception("There is no such departure point in this route");

            if (arr == null)
                throw new Exception("There is no such arrival point in this route");

            if (dep.OrderNumber >= arr.OrderNumber)
                throw new Exception("Departure point is located after arrival point");

            foreach (var rel in rels)
            {
                if (rel.OrderNumber > dep.OrderNumber &&
                    rel.OrderNumber <= arr.OrderNumber)
                {
                    adultPrice += rel.AdultPrice;
                    underagePrice += rel.UnderagePrice;
                }
            }

            // Перевод DTO, проверки

            var o = new DM.Order()
            {
                PassengerId = order.PassengerId,
                RouteId = order.RouteId,
                DeparturePoint = order.DeparturePoint,
                ArrivalPoint = order.ArrivalPoint,
                PhoneNumber = order.PhoneNumber,
                Email = order.Email,
                Date = DateTime.Now
            };

            var ts = new List<DM.Ticket>();
            int luggageSpaceTaken = 0;

            foreach (var t in tickets)
            {
                if (string.IsNullOrWhiteSpace(t.FirstName) ||
                    string.IsNullOrWhiteSpace(t.LastName) ||
                    t.Seat <= 0 ||
                    t.Luggage < 0 ||
                    t.DOB > DateTime.Today)
                    throw new Exception("Invalid ticket data");

                ts.Add(new DM.Ticket()
                {
                    FirstName = t.FirstName,
                    MiddleName = t.MiddleName,
                    LastName = t.LastName,
                    Seat = t.Seat,
                    DOB = t.DOB,
                    Luggage = t.Luggage,
                    Price = t.DOB.AddYears(18) <= DateTime.Today ? adultPrice : underagePrice
                });

                luggageSpaceTaken += t.Luggage;

                if (luggageSpaceTaken > route.LuggageSpace)
                    return MakeOrderStatus.NotEnoughLuggageSpace;
            }

            // Изменение мест
            
            var seats = tickets.Select(t => t.Seat);
            var available = dbr.Filters
                               .GetRouteSeats(order.RouteId)
                               .Where(s => s.Status == 0)
                               .ToList();

            if (available.Count == 0)
                return MakeOrderStatus.NoSeatsAvailable;

            foreach (var s in seats)
            {

                var availableSeat = available.Find(av => av.Number == s);

                if (availableSeat == null)
                {
                    return MakeOrderStatus.RequiredSeatsOccupied;
                }

                availableSeat.Status = 1;
                availableSeat.StatusChangedOn = DateTime.Now;

            }

            foreach(var s in available)
            {
                dbr.Seats.Update(s);
            }

            // Создание заказов и билетов

            dbr.Orders.Create(o);
            order.Id = o.Id;
            order.Date = o.Date;

            for (int i = 0; i < ts.Count; ++i)
            {
                ts[i].OrderId = o.Id;
                tickets[i].OrderId = o.Id;
                dbr.Tickets.Create(ts[i]);
                tickets[i].Id = ts[i].Id;
            }

            route.LuggageSpace -= luggageSpaceTaken;
            dbr.Routes.Update(route);

            dbr.Save();

            return MakeOrderStatus.Success;
        }

        public List<DTO.Order> GetOrders()
        {
            if (currentUser == null)
                throw new Exception("User is not logged in");

            return dbr.Filters.GetPassengerOrders(currentUser.Id)
                              .Select(o => new DTO.Order()
                              {
                                  Id = o.Id,
                                  RouteId = o.RouteId,
                                  DeparturePoint = o.DeparturePoint,
                                  ArrivalPoint = o.ArrivalPoint,
                                  PassengerId = o.PassengerId,
                                  PhoneNumber = o.PhoneNumber,
                                  Email = o.Email,
                                  Date = o.Date
                              }).ToList();

        }

        public List<DTO.Ticket> GetOrderTickets(int orderId)
        {

            if (dbr.Orders.Get(orderId) == null)
                throw new Exception("Order does not exist");

            return dbr.Tickets.GetAll()
                              .Where(t => t.OrderId == orderId)
                              .Select(t => new DTO.Ticket()
                              {
                                  Id = t.Id,
                                  FirstName = t.FirstName,
                                  MiddleName = t.MiddleName,
                                  LastName = t.LastName,
                                  Seat = t.Seat,
                                  DOB = t.DOB,
                                  Luggage = t.Luggage,
                                  OrderId = t.OrderId,
                                  Price = t.Price
                              })
                              .ToList();

        }

        public Dictionary<DTO.Order, List<DTO.Ticket>> GetPersonalReport(DateTime? start, DateTime? end)
        {
            if (currentUser == null)
                throw new Exception("User is not logged in");

            var orders = new List<DTO.Order>();

            if (start == null)
                start = DateTime.MinValue;

            if (end == null)
                end = DateTime.MaxValue;

            if (start == DateTime.MinValue && end == DateTime.MaxValue)
            {
                orders = dbr.Filters
                            .GetPassengerOrders(currentUser.Id)
                            .Select(o => new DTO.Order()
                            {
                                Id = o.Id,
                                RouteId = o.RouteId,
                                DeparturePoint = o.DeparturePoint,
                                ArrivalPoint = o.ArrivalPoint,
                                PassengerId = o.PassengerId,
                                PhoneNumber = o.PhoneNumber,
                                Email = o.Email,
                                Date = o.Date
                            })
                            .ToList();
            } 
            else
            {
                orders = dbr.Filters
                            .GetPassengerOrdersInPeriod(currentUser.Id, start.Value, end.Value)
                            .Select(o => new DTO.Order()
                            {
                                Id = o.Id,
                                RouteId = o.RouteId,
                                DeparturePoint = o.DeparturePoint,
                                ArrivalPoint = o.ArrivalPoint,
                                PassengerId = o.PassengerId,
                                PhoneNumber = o.PhoneNumber,
                                Email = o.Email,
                                Date = o.Date
                            })
                            .ToList();
            }

            if (orders.Count == 0)
                return new Dictionary<DTO.Order, List<DTO.Ticket>>();

            var result = new Dictionary<DTO.Order, List<DTO.Ticket>>();

            foreach (var order in orders)
            {
                var tickets = dbr.Filters
                                 .GetOrderTickets(order.Id)
                                 .Select(t => new DTO.Ticket()
                                 {
                                     Id = t.Id,
                                     FirstName = t.FirstName,
                                     MiddleName = t.MiddleName,
                                     LastName = t.LastName,
                                     Gender = t.Gender,
                                     Seat = t.Seat,
                                     DOB = t.DOB,
                                     Luggage = t.Luggage,
                                     OrderId = t.OrderId,
                                     Price = t.Price
                                 })
                                 .ToList();
                result.Add(order, tickets);
            }

            return result;

        }

        private readonly IDbRepository dbr;
        private readonly Type userType;
        private DTO.User currentUser;

    }

}