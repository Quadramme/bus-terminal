using System;

namespace Interfaces.DTO
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
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
        public DateTime RegistrationDate { get; set; }
    }

}
