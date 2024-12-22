using DomainModel;
using Interfaces.DTO;
using Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using DM = DomainModel;

namespace DAL.Repository
{

    public class FilterRepository : IFilterRepository
    {

        public FilterRepository(BusTerminalContext dbcontext)
        {
            dbc = dbcontext;
        }

        public List<DM.Rel_RouteInfo_Destination> GetRouteInfoRels(int routeInfoId)
        {
            var routeInfo = dbc.RouteInfo.Find(routeInfoId);

            if (routeInfo == null)
                return new List<DM.Rel_RouteInfo_Destination>();

            return routeInfo.Rel_RouteInfo_Destination.ToList();
        }

        public List<DM.Route> GetRouteInfoRoutes(int routeInfoId)
        {
            var routeInfo = dbc.RouteInfo.Find(routeInfoId);

            if (routeInfo == null)
                return new List<DM.Route>();

            return routeInfo.Route.ToList();
        }

        public DM.Route GetRouteInfoRoute(int routeInfoId, DateTime date)
        {
            var routeInfo = dbc.RouteInfo.Find(routeInfoId);

            if (routeInfo == null)
                return null;

            return dbc.Route.FirstOrDefault(r => r.RouteInfoId == routeInfoId 
                                                 &&
                                                 r.Date == date);
        }

        public List<DM.RouteInfo> GetRouteInfosBySchedule(int scheduleMask)
        {
            if (scheduleMask == 0)
                return new List<DM.RouteInfo>();

            return dbc.RouteInfo
                      .Where(i => (i.Schedule & scheduleMask) != 0)
                      .ToList();
        }

        public List<DM.RouteInfo> GetRouteInfosByCompany(int busCompanyId)
        {
            var busCompany = dbc.BusCompany.Find(busCompanyId);

            if (busCompany == null)
                return new List<DM.RouteInfo>();

            var theirTransport = dbc.Transport.Where(t => t.BusCompanyId == busCompanyId)
                                              .Select(t => t.Id)
                                              .ToList();

            return dbc.RouteInfo.Where(i => theirTransport.Contains(i.TransportId)).ToList();
        }

        /*public List<DM.RouteInfo> GetRouteInfoByPath(int departureId, int arrivalId)
        {
            if (departureId == 0 || arrivalId == 0)
                return new List<DM.RouteInfo>();

            var departureDestination = dbc.Destination.Find(departureId);
            var arrivalDestination = dbc.Destination.Find(arrivalId);

            if (departureDestination == null || arrivalDestination == null)
                return new List<DM.RouteInfo>();

            return dbc.Rel_RouteInfo_Destination
                      .Where(r => r.DestinationId == departureId ||
                                  r.DestinationId == arrivalId)
                      .GroupBy(r => r.RouteInfoId)
                      .Where(g => g.Count() == 2)
                      .Where(g => g.FirstOrDefault(r => r.DestinationId == departureId).OrderNumber
                                  <
                                  g.FirstOrDefault(r => r.DestinationId == arrivalId).OrderNumber)
                      .SelectMany(g => g.ToList())
                      .Select(r => r.RouteInfoId)
                      .Distinct()
                      .Select(r => r.RouteInfo)
                      .ToList();
        }*/

        public List<DM.Order> GetRouteOrders(int routeId)
        {
            var route = dbc.Route.Find(routeId);

            if (route == null)
                return new List<DM.Order>();

            return dbc.Order
                      .Where(o => o.RouteId == routeId)
                      .ToList();
        }

        public List<DM.Ticket> GetRouteTickets(int routeId)
        {
            var route = dbc.Route.Find(routeId);

            if (route == null)
                return new List<DM.Ticket>();

            return dbc.Order
                      .Where(o => o.RouteId == routeId)
                      .SelectMany(o => o.Ticket)
                      .ToList();
        }

        public List<DM.Seat> GetRouteSeats(int routeId)
        {
            var route = dbc.Route.Find(routeId);

            if (route == null)
                return new List<DM.Seat>();

            return route.Seat.ToList();
        }

        public bool CheckLogin(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
                return false;

            if (dbc.User.FirstOrDefault(u => u.Login == login) == null)
                return false;

            return true;
        }

        public DM.User GetUser(string login, string password, out Interfaces.Services.LoginStatus status)
        {
            status = Interfaces.Services.LoginStatus.Success;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                return null;

            var passenger = dbc.Passenger.FirstOrDefault(p => p.Login == login);

            if (passenger != null)
            {
                if (passenger.Password == password)
                {
                    return passenger;
                }
                else
                {
                    status = Interfaces.Services.LoginStatus.IncorrectPassword;
                    return null;
                }
            }

            var employee = dbc.Employee.FirstOrDefault(p => p.Login == login);

            if (employee != null)
            {
                if (employee.Password == password)
                {
                    return employee;
                }
                else
                {
                    status = Interfaces.Services.LoginStatus.IncorrectPassword;
                    return null;
                }
            }

            status = Interfaces.Services.LoginStatus.UserDoesNotExist;
            return null;
        }

        public List<DM.Order> GetPassengerOrders(int userId)
        {
            var passenger = dbc.Passenger.Find(userId);

            if (passenger == null)
                return new List<DM.Order>();

            return passenger.Order.ToList();
        }

        public List<DM.Ticket> GetOrderTickets(int orderId)
        {
            var order = dbc.Order.Find(orderId);

            if (order == null)
                return new List<DM.Ticket>();

            return order.Ticket.ToList();
        }

        public List<DM.Order> GetPassengerOrdersInPeriod(int passengerId, DateTime begin, DateTime end)
        {
            var passenger = dbc.Passenger.Find(passengerId);

            if (passenger == null || begin > end)
                return new List<DM.Order>();

            return passenger.Order.Where(o => o.Date >= begin && o.Date <= end)
                                  .ToList();
        }

        private readonly BusTerminalContext dbc;

    }

}