using System.Collections.Generic;

namespace DomainModel
{
    public class Destination
    {
        public int Id { get; set; }
        public int RegionId { get; set; }
        public string Type { get; set; }
        public string TypeInfo { get; set; }
        public string Settlement { get; set; }
        public string Street { get; set; }
        public string House { get; set; }

        public virtual Region Region { get; set; }
        public virtual ICollection<Rel_RouteInfo_Destination> Rel_RouteInfo_Destination { get; set; }
    }
}
