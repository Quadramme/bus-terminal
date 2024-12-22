namespace UserApp.Model
{

    public class RouteInfoCard
    {
        public int Id { get; set; }
        public string DeparturePoint { get; set; }
        public string DepartureTime { get; set; }
        public string DepartureDate { get; set; }
        public string TravelTime { get; set; }
        public string ArrivalPoint { get; set; }
        public string ArrivalTime { get; set; }
        public string ArrivalDate { get; set; }
        public string BusCompany { get; set; }
        public string Schedule { get; set; }
        public string AdultPrice { get; set; }
        public string UnderagePrice { get; set; }
    }

}