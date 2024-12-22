using System.Collections.Generic;

namespace Interfaces.DTO
{

    public class Passenger : User
    {

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public List<int> PaymentIds { get; set; }

        public List<int> OrderIds { get; set; }

    }

}
