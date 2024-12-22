using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM = DomainModel;

namespace Interfaces.Repository
{
    public interface IFilterRepository
    {

        // Routes

        List<DM.Rel_RouteInfo_Destination> GetRouteInfoRels(int routeInfoId);

        List<DM.Route> GetRouteInfoRoutes(int routeInfoId);

        DM.Route GetRouteInfoRoute(int routeInfoId, DateTime date);

        List<DM.RouteInfo> GetRouteInfosBySchedule(int scheduleMask);

        List<DM.RouteInfo> GetRouteInfosByCompany(int busCompanyId);

        List<DM.Order> GetRouteOrders(int routeId);

        List<DM.Ticket> GetRouteTickets(int routeId);

        List<DM.Seat> GetRouteSeats(int routeId);

        // Users

        bool CheckLogin(string login);

        DM.User GetUser(string login, string password, out Interfaces.Services.LoginStatus status);

        List<DM.Order> GetPassengerOrders(int passengerId);

        List<DM.Ticket> GetOrderTickets(int orderId);

        List<DM.Order> GetPassengerOrdersInPeriod(int passengerId, DateTime begin, DateTime end);

    }

}
