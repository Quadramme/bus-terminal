using DomainModel;

namespace Interfaces.Repository
{

    public interface IDbRepository
    {

        IRepository<BusCompany> BusCompanies { get; }
        IRepository<Driver> Drivers { get; }
        IRepository<Transport> Transports { get; }
        IRepository<TransportType> TransportTypes { get; }
        IRepository<Destination> Destinations { get; }
        IRepository<Region> Regions { get; }
        IRepository<Rel_RouteInfo_Destination> Rels_RouteInfo_Destination { get; }
        IRepository<RouteInfo> RouteInfos { get; }
        IRepository<Route> Routes { get; }
        IRepository<Seat> Seats { get; }
        IRepository<RouteStatus> RouteStatuses { get; }
        IRepository<User> Users { get; }
        IRepository<Employee> Employees { get; }
        IRepository<EmployeeType> EmployeeTypes { get; }
        IRepository<Passenger> Passengers { get; }
        IRepository<PaymentInfo> PaymentInfos { get; }
        IRepository<Order> Orders { get; }
        IRepository<Ticket> Tickets { get; }
        IFilterRepository Filters { get; }

        bool CanConnect();
        int Save();

    }

}