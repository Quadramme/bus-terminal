namespace DomainModel
{
    public class Employee : User
    {
        public int EmployeeTypeId { get; set; }

        public virtual EmployeeType EmployeeType { get; set; }
    }

}
