using System.Collections.Generic;

namespace DomainModel
{
    public class Passenger : User
    {
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public virtual ICollection<PaymentInfo> PaymentInfo { get; set; }
        public virtual ICollection<Order> Order { get; set; }
    }

}
