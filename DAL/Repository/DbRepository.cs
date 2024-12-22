using DomainModel;
using Interfaces.Repository;
using System.Data.SqlClient;

namespace DAL.Repository
{

    public class DbRepository : IDbRepository
    {

        public DbRepository(string connectionString) 
        {
            dbc = new BusTerminalContext(connectionString);
            busCompanyRepo = new BusCompanyRepository(dbc);
            driverRepo = new DriverRepository(dbc);
            transportRepo = new TransportRepository(dbc);
            transportTypeRepo = new TransportTypeRepository(dbc);
            destinationRepo = new DestinationRepository(dbc);
            regionRepo = new RegionRepository(dbc);
            rel_routeinfo_destination_Repo = new Rel_RouteInfo_Destination_Repository(dbc);
            routeInfoRepo = new RouteInfoRepository(dbc);
            routeRepo = new RouteRepository(dbc);
            seatRepo = new SeatRepository(dbc);
            routeStatusRepo = new RouteStatusRepository(dbc);
            userRepo = new UserRepository(dbc);
            employeeRepo = new EmployeeRepository(dbc);
            employeeTypeRepo = new EmployeeTypeRepository(dbc);
            passengerRepo = new PassengerRepository(dbc);
            paymentInfoRepo = new PaymentInfoRepository(dbc);
            orderRepo = new OrderRepository(dbc);
            ticketRepo = new TicketRepository(dbc);
            filterRepo = new FilterRepository(dbc);
        }

        public IRepository<BusCompany> BusCompanies { get => busCompanyRepo; }

        public IRepository<Driver> Drivers { get => driverRepo; }

        public IRepository<Transport> Transports { get => transportRepo; }

        public IRepository<TransportType> TransportTypes { get => transportTypeRepo; }

        public IRepository<Destination> Destinations { get => destinationRepo; }

        public IRepository<Region> Regions { get => regionRepo; }

        public IRepository<Rel_RouteInfo_Destination> Rels_RouteInfo_Destination { get => rel_routeinfo_destination_Repo; }

        public IRepository<RouteInfo> RouteInfos { get => routeInfoRepo; }

        public IRepository<Route> Routes { get => routeRepo; }

        public IRepository<Seat> Seats { get => seatRepo; }

        public IRepository<RouteStatus> RouteStatuses { get => routeStatusRepo; }

        public IRepository<User> Users { get => userRepo; }

        public IRepository<Employee> Employees { get => employeeRepo; }

        public IRepository<EmployeeType> EmployeeTypes { get => employeeTypeRepo; }

        public IRepository<Passenger> Passengers { get => passengerRepo; }

        public IRepository<PaymentInfo> PaymentInfos { get => paymentInfoRepo; }

        public IRepository<Order> Orders { get => orderRepo; }

        public IRepository<Ticket> Tickets { get => ticketRepo; }

        public IFilterRepository Filters { get => filterRepo; }

        public bool CanConnect()
        {
            try
            {
                dbc.Database.Connection.Open();
                dbc.Database.Connection.Close();
            }
            catch (SqlException)
            {
                return false;
            }
            return true;
        }

        public int Save() { return dbc.SaveChanges(); }

        private readonly BusTerminalContext dbc;

        private readonly BusCompanyRepository busCompanyRepo;
        private readonly DriverRepository driverRepo;
        private readonly TransportRepository transportRepo;
        private readonly TransportTypeRepository transportTypeRepo;
        private readonly DestinationRepository destinationRepo;
        private readonly RegionRepository regionRepo;
        private readonly Rel_RouteInfo_Destination_Repository rel_routeinfo_destination_Repo;
        private readonly RouteInfoRepository routeInfoRepo;
        private readonly RouteRepository routeRepo;
        private readonly SeatRepository seatRepo;
        private readonly RouteStatusRepository routeStatusRepo;
        private readonly UserRepository userRepo;
        private readonly EmployeeRepository employeeRepo;
        private readonly EmployeeTypeRepository employeeTypeRepo;
        private readonly PassengerRepository passengerRepo;
        private readonly PaymentInfoRepository paymentInfoRepo;
        private readonly OrderRepository orderRepo;
        private readonly TicketRepository ticketRepo;
        private readonly FilterRepository filterRepo;

    }

}