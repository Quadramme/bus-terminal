using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserApp.Model
{
    public class ReportPositionData
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal AdultPrice { get; set; }
        public int AdultCount { get; set; }
        public decimal UnderagePrice { get; set; }
        public int UnderageCount { get; set; }
        public decimal TotalPrice { get => AdultCount * AdultPrice + UnderageCount * UnderagePrice; }
        public int RouteInfoId { get; set; }
        public int RouteId { get; set; }
        public string DepartureCity { get; set; }
        public DateTime DepartureDate { get; set; }
        public string ArrivalCity { get; set; }
        public DateTime ArrivalDate { get; set; }
        public TimeSpan TravelTime { get; set; }
    }

    public class ReportData
    {
        public decimal OverallPrice { get; set; } = 0m;
        public TimeSpan OverallTravelTime { get; set; } = TimeSpan.Zero;
        public int MostUsedRouteInfoId { get; set; } = 0;
        public int MostUsedOrderCount { get; set; } = 0;
        public decimal MostUsedSpent { get; set; } = 0m;
        public string MostUsedDeparture { get; set; } = "";
        public string MostUsedArrival { get; set; } = "";
        public string MostUsedCompany { get; set; } = "";
    }
}