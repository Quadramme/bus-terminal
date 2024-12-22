using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserApp.Model
{
    public class Order
    {

        public int Id { get; set; }

        public int RouteId { get; set; }

        public string DepartureCity { get; set; }

        public string DepartureInfo { get; set; }

        public DateTime DepartureDate { get; set; }

        public int? DeparturePlatform { get; set; }

        public string ArrivalCity { get; set; }

        public string ArrivalInfo { get; set; }

        public DateTime ArrivalDate { get; set; }

        public int? ArrivalPlatform { get; set; }

        public DateTime Date { get; set; }

        public decimal Price { get; set; }

    }

}