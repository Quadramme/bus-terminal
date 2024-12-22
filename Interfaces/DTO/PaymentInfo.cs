using System;

namespace Interfaces.DTO
{
    public class PaymentInfo
    {

        public int Id { get; set; }

        public string PAN { get; set; }

        public DateTime ExpirationDate { get; set; }

        public int PassengerId { get; set; }

    }

}
