using DTO = Interfaces.DTO;
using System;
using System.Collections.Generic;

namespace Interfaces.Services
{

    public interface IRouteService : IService
    {

        // CRUD RouteInfo

        DTO.RouteInfo GetRouteInfoById(int id);
        List<DTO.RouteInfo> GetAllRouteInfos();
        void CreateRouteInfo(DTO.RouteInfo info, IEnumerable<DTO.Rel_RouteInfo_Destination> rels);
        void DeleteRouteInfo(int id);

        // CRUD Route / Seats

        void CreateRoute(int routeInfoId, DateTime date);

        // CRUD Destination 

        DTO.Destination GetDestinationById(int id);
        List<DTO.Destination> GetAllDestinations();
        void CreateDestination(DTO.Destination d);
        void DeleteDestination(int id);

        // CRUD Transport

        DTO.Transport GetTransportById(int id);
        List<DTO.Transport> GetAllTransports();
        void CreateTransport(DTO.Transport t);
        void DeleteTransport(int id);

        // CRUD Driver

        DTO.Driver GetDriverById(int id);
        List<DTO.Driver> GetAllDrivers();
        void CreateDriver(DTO.Driver d);
        void DeleteDriver(int id);

        // CRUD BusCompany

        DTO.BusCompany GetBusCompanyById(int id);
        List<DTO.BusCompany> GetAllBusCompanies();
        void CreateBusCompany(DTO.BusCompany bc);
        void DeleteBusCompany(int id);

        // CRUD TransportType

        DTO.TransportType GetTransportTypeById(int id);

        // CRUD Region

        DTO.Region GetRegionById(int id);

        // Route search

        int DateToScheduleMask(DateTime date);

        List<string> ScheduleMaskToWeekdays(int mask);

        List<DTO.RouteInfo> FindRoutes(int departureId, int arrivalId, DateTime date);

        List<DTO.RouteInfo> FindRoutesByCompany(int busCompanyId);

        List<DTO.Destination> GetFullRouteInfo(
            int routeInfoId,
            int? departureId,
            int? arrivalId,
            out decimal adultPrice,
            out decimal underagePrice,
            out int travelHours,
            out byte travelMinutes,
            out TimeSpan departureTime,
            out TimeSpan arrivalTime);

        List<DTO.Route> GetRouteInfoRoutes(int routeInfoId);

        DTO.RouteInfo GetRouteInfo(int routeId);

        // Get route information

        List<DateTime> GetRouteTimes(int routeId);
        void GetRouteStatus(int routeInfoId, DateTime date, out int routeId, out int status);
        List<int> GetFreeRouteSeats(int routeId);
        int GetFreeLuggageSpace(int routeId);
        void GetRoutePlatforms(int routeId, int? departureId, int? arrivalId, out int? departurePlatform, out int? arrivalPlatform);
        void GetRouteInfoPlatforms(int routeInfoId, out int? departurePlatform, out int? arrivalPlatform);
        void GetRouteArrivalDepartureTime(int routeId, int departureId, int arrivalId, out DateTime departureTime, out DateTime arrivalTime);

    }

}