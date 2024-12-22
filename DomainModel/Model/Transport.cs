using System.Collections.Generic;

namespace DomainModel
{
    public class Transport
    {
        public int Id { get; set; }
        public int TransportTypeId { get; set; }
        public int DriverId { get; set; }
        public int BusCompanyId { get; set; }
        public string RegistrationNumber { get; set; }
        public int MaxSeats { get; set; }
        public int MaxLuggage { get; set; }

        public virtual TransportType TransportType { get; set; }
        public virtual Driver Driver { get; set; }
        public virtual BusCompany BusCompany { get; set; }
        public virtual ICollection<RouteInfo> RouteInfo { get; set; }
    }
}
