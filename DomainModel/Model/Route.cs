using System;
using System.Collections.Generic;

namespace DomainModel
{

    public class Route
    {
        public int Id { get; set; }
        public int RouteInfoId { get; set; }
        public int LuggageSpace { get; set; }
        public DateTime Date { get; set; }
        public int RouteStatusId { get; set; }

        public virtual ICollection<Order> Order { get; set; }
        public virtual RouteInfo RouteInfo { get; set; }
        public virtual RouteStatus RouteStatus { get; set; }
        public virtual ICollection<Seat> Seat { get; set; }
    }

}
