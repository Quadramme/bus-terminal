namespace Interfaces.DTO
{
    public class Rel_RouteInfo_Destination
    {
        public int RouteInfoId { get; set; }
        public int DestinationId { get; set; }
        public int OrderNumber { get; set; }
        public decimal AdultPrice { get; set; }
        public decimal UnderagePrice { get; set; }
        public int ArrivalHour { get; set; }
        public byte ArrivalMinute { get; set; }
        public int? ArrivalPlatform { get; set; }
    }

}
