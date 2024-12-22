using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApp.Model
{
    public class Destination
    {

        public int Id { get; set; }

        public string Region { get; set; }

        public string Type { get; set; }

        public string TypeInfo { get; set; }

        public string Settlement { get; set; }

        public string Street { get; set; }

        public string House { get; set; }

        public DateTime ArrivalTime { get; set; }

        public bool IsVisited { get; set; }

        public string Name
        {

            get
            {

                if (TypeInfo == null)
                    return $"{Settlement.Substring(Settlement.IndexOf(". ") + 2)} {Type}";

                return $"{Settlement.Substring(Settlement.IndexOf(". ") + 2)} {Type} {TypeInfo}";

            }

        }

        public string SettlementName
        {
            get => Settlement.Substring(Settlement.IndexOf(". ") + 2);
        }

    }

}