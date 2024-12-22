using System;
using System.Collections.Generic;

namespace DomainModel
{

    public class RouteInfo
    {
        public int Id { get; set; }
        public int TransportId { get; set; }
        public int Schedule { get; set; }

        public virtual ICollection<Rel_RouteInfo_Destination> Rel_RouteInfo_Destination { get; set; }
        public virtual ICollection<Route> Route { get; set; }
        public virtual Transport Transport { get; set; }
    }

}
