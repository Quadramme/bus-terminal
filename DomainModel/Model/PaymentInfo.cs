using System;
using System.Collections.Generic;

namespace DomainModel
{
    public class PaymentInfo
    {
        public int Id { get; set; }
        public string PAN { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int PassengerId { get; set; }

        public virtual Passenger Passenger { get; set; }
    }

}
