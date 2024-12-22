namespace Interfaces.DTO
{
    public class Transport
    {

        public int Id { get; set; }

        public string RegistrationNumber { get; set; }

        public int MaxSeats { get; set; }

        public int MaxLuggage { get; set; }

        public int BusCompanyId { get; set; }

        public int DriverId { get; set; }

        public int TransportTypeId { get; set; }

    }

}
