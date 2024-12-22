using System.Collections.Generic;
using System.Linq;

namespace Interfaces.DTO
{
    public class Driver
    {

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string FullName
        {

            get
            {

                if (MiddleName == null)
                    return $"{LastName} {FirstName}";

                return $"{LastName} {FirstName} {MiddleName}";

            }

        }

        public string ShortName
        {
            get 
            {

                if (MiddleName == null)
                    return $"{LastName} {FirstName.First()}.";

                return $"{LastName} {FirstName.First()}.{MiddleName.First()}.";

            }
        }

        public List<int> TransportIds { get; set; }

    }

}
