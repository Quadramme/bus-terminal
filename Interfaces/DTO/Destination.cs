using System.Collections.Generic;
using System.Xml.Linq;

namespace Interfaces.DTO
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
        public string Name
        {

            get {

                if (TypeInfo == null)
                    return $"{Settlement.Substring(Settlement.IndexOf(". ") + 2)} {Type}";

                return $"{Settlement.Substring(Settlement.IndexOf(". ") + 2)} {Type} {TypeInfo}";
                    
             }
        }
        public string SettlementName
        {
            get => Settlement.Substring(Settlement.IndexOf(". ") + 2);
        }

        //public List<int> Rel_RouteInfo_Destination_Ids { get; set; }

    }

}
