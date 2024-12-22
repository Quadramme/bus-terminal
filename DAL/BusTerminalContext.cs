using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using DomainModel;

namespace DAL
{
    public partial class BusTerminalContext : DbContext
    {
        public BusTerminalContext(string connectionString)
        : base(connectionString)
        { }

        public virtual DbSet<BusCompany> BusCompany { get; set; }
        public virtual DbSet<Destination> Destination { get; set; }
        public virtual DbSet<Driver> Driver { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<EmployeeType> EmployeeType { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<Passenger> Passenger { get; set; }
        public virtual DbSet<PaymentInfo> PaymentInfo { get; set; }
        public virtual DbSet<Region> Region { get; set; }
        public virtual DbSet<Rel_RouteInfo_Destination> Rel_RouteInfo_Destination { get; set; }
        public virtual DbSet<Route> Route { get; set; }
        public virtual DbSet<RouteInfo> RouteInfo { get; set; }
        public virtual DbSet<RouteStatus> RouteStatus { get; set; }
        public virtual DbSet<Seat> Seat { get; set; }
        public virtual DbSet<Ticket> Ticket { get; set; }
        public virtual DbSet<Transport> Transport { get; set; }
        public virtual DbSet<TransportType> TransportType { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            #region BusCompany Entity

            var BusCompanyConfig = modelBuilder.Entity<BusCompany>();

            BusCompanyConfig
                .ToTable("bus_company")
                .HasKey(e => e.Id);

            BusCompanyConfig
                .Property(e => e.Id)
                .HasColumnName("id");

            BusCompanyConfig
                .Property(e => e.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(128)
                .IsUnicode(false);

            BusCompanyConfig
                .HasMany(e => e.Transport)
                .WithRequired(e => e.BusCompany)
                .HasForeignKey(e => e.BusCompanyId)
                .WillCascadeOnDelete(false);

            #endregion

            #region Destination Entity

            var DestinationConfig = modelBuilder.Entity<Destination>();

            DestinationConfig
                .ToTable("destination")
                .HasKey(e => e.Id);

            DestinationConfig
                .Property(e => e.Id)
                .HasColumnName("id");

            DestinationConfig
                .Property(e => e.RegionId)
                .HasColumnName("region_id");

            DestinationConfig
                .Property(e => e.Type)
                .HasColumnName("type")
                .HasMaxLength(2)
                .IsUnicode(false);

            DestinationConfig
                .Property(e => e.TypeInfo)
                .HasColumnName("type_info")
                .HasMaxLength(64)
                .IsUnicode(false);

            DestinationConfig
                .Property(e => e.Settlement)
                .HasColumnName("settlement")
                .IsRequired()
                .HasMaxLength(64)
                .IsUnicode(false);

            DestinationConfig
                .Property(e => e.Street)
                .HasColumnName("street")
                .IsRequired()
                .HasMaxLength(64)
                .IsUnicode(false);

            DestinationConfig
                .Property(e => e.House)
                .HasMaxLength(8)
                .IsUnicode(false);

            DestinationConfig
                .HasMany(e => e.Rel_RouteInfo_Destination)
                .WithRequired(e => e.Destination)
                .HasForeignKey(e => e.DestinationId)
                .WillCascadeOnDelete(false);

            #endregion

            #region Driver Entity

            var DriverConfig = modelBuilder.Entity<Driver>();

            DriverConfig
                .ToTable("driver")
                .HasKey(e => e.Id);

            DriverConfig
                .Property(e => e.Id)
                .HasColumnName("id");

            DriverConfig
                .Property(e => e.FirstName)
                .HasColumnName("first_name")
                .IsRequired()
                .HasMaxLength(64)
                .IsUnicode(false);

            DriverConfig
                .Property(e => e.MiddleName)
                .HasColumnName("middle_name")
                .HasMaxLength(64)
                .IsUnicode(false);

            DriverConfig
                .Property(e => e.LastName)
                .HasColumnName("last_name")
                .IsRequired()
                .HasMaxLength(64)
                .IsUnicode(false);

            DriverConfig
                .HasMany(e => e.Transport)
                .WithRequired(e => e.Driver)
                .HasForeignKey(e => e.DriverId)
                .WillCascadeOnDelete(false);

            #endregion

            #region EmployeeType Entity

            var EmployeeTypeConfig = modelBuilder.Entity<EmployeeType>();

            EmployeeTypeConfig
                .ToTable("employee_type")
                .HasKey(e => e.Id);

            EmployeeTypeConfig
                .Property(e => e.Id)
                .HasColumnName("id");

            EmployeeTypeConfig
                .Property(e => e.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(32)
                .IsUnicode(false);

            EmployeeTypeConfig
                .HasMany(e => e.Employee)
                .WithRequired(e => e.EmployeeType)
                .HasForeignKey(e => e.EmployeeTypeId)
                .WillCascadeOnDelete(false);

            #endregion

            #region Order Entity

            var OrderConfig = modelBuilder.Entity<Order>();

            OrderConfig
                .ToTable("order")
                .HasKey(e => e.Id);

            OrderConfig
                .Property(e => e.Id)
                .HasColumnName("id");

            OrderConfig
                .Property(e => e.PassengerId)
                .HasColumnName("passenger_id");

            OrderConfig
                .Property(e => e.RouteId)
                .HasColumnName("route_id")
                .IsRequired();

            OrderConfig
                .Property(e => e.DeparturePoint)
                .HasColumnName("departure_point")
                .IsRequired();

            OrderConfig
                .Property(e => e.ArrivalPoint)
                .HasColumnName("arrival_point")
                .IsRequired();

            OrderConfig
                .Property(e => e.PhoneNumber)
                .HasColumnName("phone_number")
                .IsRequired()
                .HasMaxLength(15)
                .IsUnicode(false);

            OrderConfig
                .Property(e => e.Email)
                .HasColumnName("email")
                .IsRequired()
                .HasMaxLength(64)
                .IsUnicode(false);

            OrderConfig
                .Property(e => e.Date)
                .HasColumnName("date")
                .HasColumnType("date")
                .IsRequired();

            OrderConfig
                .HasMany(e => e.Ticket)
                .WithRequired(e => e.Order)
                .HasForeignKey(e => e.OrderId)
                .WillCascadeOnDelete(false);

            #endregion

            #region PaymentInfo Entity

            var PaymentInfoConfig = modelBuilder.Entity<PaymentInfo>();

            PaymentInfoConfig
                .ToTable("payment_info")
                .HasKey(e => e.Id);

            PaymentInfoConfig
                .Property(e => e.Id)
                .HasColumnName("id");

            PaymentInfoConfig
                .Property(e => e.PassengerId)
                .HasColumnName("passenger_id");

            PaymentInfoConfig
                .Property(e => e.PAN)
                .HasColumnName("pan")
                .IsRequired()
                .HasMaxLength(19)
                .IsUnicode(false);

            PaymentInfoConfig
                .Property(e => e.ExpirationDate)
                .HasColumnName("expiration_date")
                .HasColumnType("date")
                .IsRequired();

            PaymentInfoConfig
                .HasRequired(e => e.Passenger)
                .WithMany(e => e.PaymentInfo)
                .WillCascadeOnDelete(false);

            #endregion

            #region Region Entity

            var RegionConfig = modelBuilder.Entity<Region>();

            RegionConfig
                .ToTable("region")
                .HasKey(e => e.Id);

            RegionConfig
                .Property(e => e.Id)
                .HasColumnName("id");

            RegionConfig
                .Property(e => e.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(64)
                .IsUnicode(false);

            modelBuilder.Entity<Region>()
                .HasMany(e => e.Destination)
                .WithRequired(e => e.Region)
                .HasForeignKey(e => e.RegionId)
                .WillCascadeOnDelete(false);

            #endregion

            #region Rel_RouteInfo_Destination Entity

            var RelConfig = modelBuilder.Entity<Rel_RouteInfo_Destination>();

            RelConfig
                .ToTable("rel_route_info_destination")
                .HasKey(e => new { e.RouteInfoId, e.DestinationId, e.OrderNumber });

            RelConfig
                .Property(e => e.RouteInfoId)
                .HasColumnName("route_info_id")
                .HasColumnOrder(0)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            RelConfig
                .Property(e => e.DestinationId)
                .HasColumnName("destination_id")
                .HasColumnOrder(1)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            RelConfig
                .Property(e => e.OrderNumber)
                .HasColumnName("order_number")
                .HasColumnOrder(2)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            RelConfig
                .Property(e => e.AdultPrice)
                .HasColumnName("adult_price")
                .HasColumnType("money")
                .IsRequired()
                .HasPrecision(19, 4);

            RelConfig
                .Property(e => e.UnderagePrice)
                .HasColumnName("underage_price")
                .HasColumnType("money")
                .IsRequired()
                .HasPrecision(19, 4);

            RelConfig
                .Property(e => e.ArrivalHour)
                .HasColumnName("arrival_hour")
                .IsRequired();

            RelConfig
                .Property(e => e.ArrivalMinute)
                .HasColumnName("arrival_minute")
                .HasColumnType("tinyint")
                .IsRequired();

            RelConfig
                .Property(e => e.ArrivalPlatform)
                .HasColumnName("arrival_platform")
                .IsOptional();

            #endregion

            #region Route Entity

            var RouteConfig = modelBuilder.Entity<Route>();

            RouteConfig
                .ToTable("route")
                .HasKey(e => e.Id);

            RouteConfig
                .Property(e => e.Id)
                .HasColumnName("id");

            RouteConfig
                .Property(e => e.RouteInfoId)
                .HasColumnName("route_info_id")
                .IsRequired();

            RouteConfig
                .Property(e => e.LuggageSpace)
                .HasColumnName("luggage_space")
                .IsRequired();

            RouteConfig
                .Property(e => e.Date)
                .HasColumnName("date")
                .HasColumnType("date")
                .IsRequired();

            RouteConfig
                .Property(e => e.RouteStatusId)
                .HasColumnName("route_status_id")
                .IsRequired();

            RouteConfig
                .HasMany(e => e.Order)
                .WithRequired(e => e.Route)
                .HasForeignKey(e => e.RouteId)
                .WillCascadeOnDelete(false);

            RouteConfig
                .HasMany(e => e.Seat)
                .WithRequired(e => e.Route)
                .HasForeignKey(e => e.RouteId)
                .WillCascadeOnDelete(false);

            #endregion

            #region Seat Entity

            var SeatConfig = modelBuilder.Entity<Seat>();

            SeatConfig
                .ToTable("seat")
                .HasKey(e => new { e.Number, e.RouteId });

            SeatConfig
                .Property(e => e.Number)
                .HasColumnName("number")
                .HasColumnOrder(0)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            SeatConfig
                .Property(e => e.RouteId)
                .HasColumnName("route_id")
                .HasColumnOrder(1)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            SeatConfig
                .Property(e => e.Status)
                .HasColumnName("status");
				
			SeatConfig
				.Property(e => e.StatusChangedOn)
				.HasColumnName("status_changed_on");

            #endregion

            #region RouteInfo Entity

            var RouteInfoConfig = modelBuilder.Entity<RouteInfo>();

            RouteInfoConfig
                .ToTable("route_info")
                .HasKey(e => e.Id);

            RouteInfoConfig
                .Property(e => e.Id)
                .HasColumnName("id");

            RouteInfoConfig
                .Property(e => e.TransportId)
                .HasColumnName("transport_id")
                .IsRequired();

            RouteInfoConfig
                .Property(e => e.Schedule)
                .HasColumnName("schedule");

            RouteInfoConfig
                .HasMany(e => e.Rel_RouteInfo_Destination)
                .WithRequired(e => e.RouteInfo)
                .HasForeignKey(e => e.RouteInfoId)
                .WillCascadeOnDelete(false);

            RouteInfoConfig
                .HasMany(e => e.Route)
                .WithRequired(e => e.RouteInfo)
                .HasForeignKey(e => e.RouteInfoId)
                .WillCascadeOnDelete(false);

            #endregion

            #region RouteStatus Entity


            var RouteStatusConfig = modelBuilder.Entity<RouteStatus>();

            RouteStatusConfig
                .ToTable("route_status")
                .HasKey(e => e.Id);

            RouteStatusConfig
                .Property(e => e.Id)
                .HasColumnName("id");

            RouteStatusConfig
                .Property(e => e.Name)
                .HasColumnName("name")
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(10);

            modelBuilder.Entity<RouteStatus>()
                .HasMany(e => e.Route)
                .WithRequired(e => e.RouteStatus)
                .HasForeignKey(e => e.RouteStatusId)
                .WillCascadeOnDelete(false);

            #endregion

            #region Ticket Entity

            var TicketConfig = modelBuilder.Entity<Ticket>();

            TicketConfig
                .ToTable("ticket")
                .HasKey(e => e.Id);

            TicketConfig
                .Property(e => e.Id)
                .HasColumnName("id");

            TicketConfig
                .Property(e => e.OrderId)
                .HasColumnName("order_id")
                .IsRequired();

            TicketConfig
                .Property(e => e.FirstName)
                .HasColumnName("first_name")
                .IsRequired()
                .HasMaxLength(64)
                .IsUnicode(false);

            TicketConfig
                .Property(e => e.MiddleName)
                .HasColumnName("middle_name")
                .HasMaxLength(64)
                .IsUnicode(false);

            TicketConfig
                .Property(e => e.LastName)
                .HasColumnName("last_name")
                .IsRequired()
                .HasMaxLength(64)
                .IsUnicode(false);

            TicketConfig
                .Property(e => e.Gender)
                .HasColumnName("gender")
                .IsRequired();

            TicketConfig
                .Property(e => e.Seat)
                .HasColumnName("seat")
                .IsRequired();

            TicketConfig
                .Property(e => e.DOB)
                .HasColumnName("dob")
                .HasColumnType("date")
                .IsRequired();

            TicketConfig
                .Property(e => e.Luggage)
                .HasColumnName("luggage")
                .IsRequired();

            TicketConfig
                .Property(e => e.Price)
                .HasColumnName("price")
                .IsRequired()
                .HasColumnType("money")
                .HasPrecision(19, 4);

            #endregion

            #region Transport Entity

            var TransportConfig = modelBuilder.Entity<Transport>();

            TransportConfig
                .ToTable("transport")
                .HasKey(e => e.Id);

            TransportConfig
                .Property(e => e.Id)
                .HasColumnName("id");

            TransportConfig
                .Property(e => e.TransportTypeId)
                .HasColumnName("transport_type_id")
                .IsRequired();

            TransportConfig
                .Property(e => e.DriverId)
                .HasColumnName("driver_id")
                .IsRequired();

            TransportConfig
                .Property(e => e.BusCompanyId)
                .HasColumnName("bus_company_id")
                .IsRequired();

            TransportConfig
                .Property(e => e.RegistrationNumber)
                .HasColumnName("registration_number")
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8);

            TransportConfig
                .Property(e => e.MaxSeats)
                .HasColumnName("max_seats")
                .IsRequired();

            TransportConfig
                .Property(e => e.MaxLuggage)
                .HasColumnName("max_luggage")
                .IsRequired();

            TransportConfig
                .HasMany(e => e.RouteInfo)
                .WithRequired(e => e.Transport)
                .HasForeignKey(e => e.TransportId)
                .WillCascadeOnDelete(false);

            #endregion

            #region TransportType Entity

            var TransportTypeConfig = modelBuilder.Entity<TransportType>();

            TransportTypeConfig
                .ToTable("transport_type")
                .HasKey(e => e.Id);

            TransportTypeConfig
                .Property(e => e.Id)
                .HasColumnName("id");

            TransportTypeConfig
                .Property(e => e.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(32)
                .IsUnicode(false);

            modelBuilder.Entity<TransportType>()
                .HasMany(e => e.Transport)
                .WithRequired(e => e.TransportType)
                .HasForeignKey(e => e.TransportTypeId)
                .WillCascadeOnDelete(false);

            #endregion

            #region User Entity

            var UserConfig = modelBuilder.Entity<User>();

            UserConfig
                .ToTable("user");
                /*.HasKey(e => e.Id);

            UserConfig
                .Property(e => e.Id)
                .HasColumnName("id");

            UserConfig
                .Property(e => e.Login)
                .HasColumnName("login")
                .IsRequired()
                .HasMaxLength(64)
                .IsUnicode(false);

            UserConfig
                .Property(e => e.Password)
                .HasColumnName("password")
                .IsRequired()
                .HasMaxLength(64)
                .IsUnicode(false);

            UserConfig
                .Property(e => e.FirstName)
                .HasColumnName("first_name")
                .IsRequired()
                .HasMaxLength(64)
                .IsUnicode(false);

            UserConfig
                .Property(e => e.MiddleName)
                .HasColumnName("middle_name")
                .HasMaxLength(64)
                .IsUnicode(false);

            UserConfig
                .Property(e => e.LastName)
                .HasColumnName("last_name")
                .IsRequired()
                .HasMaxLength(64)
                .IsUnicode(false);

            UserConfig
                .Property(e => e.RegistrationDate)
                .HasColumnName("registration_date")
                .HasColumnType("date")
                .IsRequired();*/

            #endregion

            #region Passenger Entity

            var PassengerConfig = modelBuilder.Entity<Passenger>();

            PassengerConfig
                .ToTable("passenger")
                .HasKey(e => e.Id);

            PassengerConfig
                .Property(e => e.Id)
                .HasColumnName("id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            PassengerConfig
                .Property(e => e.Login)
                .HasColumnName("login")
                .IsRequired()
                .HasMaxLength(64)
                .IsUnicode(false);

            PassengerConfig
                .Property(e => e.Password)
                .HasColumnName("password")
                .IsRequired()
                .HasMaxLength(64)
                .IsUnicode(false);

            PassengerConfig
                .Property(e => e.FirstName)
                .HasColumnName("first_name")
                .IsRequired()
                .HasMaxLength(64)
                .IsUnicode(false);

            PassengerConfig
                .Property(e => e.MiddleName)
                .HasColumnName("middle_name")
                .HasMaxLength(64)
                .IsUnicode(false);

            PassengerConfig
                .Property(e => e.LastName)
                .HasColumnName("last_name")
                .IsRequired()
                .HasMaxLength(64)
                .IsUnicode(false);

            PassengerConfig
                .Property(e => e.RegistrationDate)
                .HasColumnName("registration_date")
                .HasColumnType("date")
                .IsRequired();

            PassengerConfig
                .Property(e => e.PhoneNumber)
                .HasColumnName("phone_number")
                .HasMaxLength(15)
                .IsUnicode(false);

            PassengerConfig
                .Property(e => e.Email)
                .HasColumnName("email")
                .HasMaxLength(64)
                .IsUnicode(false);

            PassengerConfig
                .HasMany(e => e.PaymentInfo)
                .WithRequired(e => e.Passenger)
                .HasForeignKey(e => e.PassengerId);

            PassengerConfig
                .HasMany(e => e.Order)
                .WithOptional(e => e.Passenger)
                .HasForeignKey(e => e.PassengerId)
                .WillCascadeOnDelete(false);

            #endregion

            #region Employee Entity

            var EmployeeConfig = modelBuilder.Entity<Employee>();

            EmployeeConfig
                .ToTable("employee")
                .HasKey(e => e.Id);

            EmployeeConfig
                .Property(e => e.Id)
                .HasColumnName("id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            EmployeeConfig
                .Property(e => e.Login)
                .HasColumnName("login")
                .IsRequired()
                .HasMaxLength(64)
                .IsUnicode(false);

            EmployeeConfig
                .Property(e => e.Password)
                .HasColumnName("password")
                .IsRequired()
                .HasMaxLength(64)
                .IsUnicode(false);

            EmployeeConfig
                .Property(e => e.FirstName)
                .HasColumnName("first_name")
                .IsRequired()
                .HasMaxLength(64)
                .IsUnicode(false);

            EmployeeConfig
                .Property(e => e.MiddleName)
                .HasColumnName("middle_name")
                .HasMaxLength(64)
                .IsUnicode(false);

            EmployeeConfig
                .Property(e => e.LastName)
                .HasColumnName("last_name")
                .IsRequired()
                .HasMaxLength(64)
                .IsUnicode(false);

            EmployeeConfig
                .Property(e => e.RegistrationDate)
                .HasColumnName("registration_date")
                .HasColumnType("date")
                .IsRequired();

            EmployeeConfig
                .Property(e => e.EmployeeTypeId)
                .HasColumnName("employee_type_id")
                .IsRequired();

            EmployeeConfig
                .HasRequired(e => e.EmployeeType)
                .WithMany(e => e.Employee)
                .WillCascadeOnDelete(false);

            #endregion

        }

    }

}