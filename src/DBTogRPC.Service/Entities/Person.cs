using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Person
    {
        [Key]
        public string PersonKey { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public Address BillingAddress { get; set; }
    }
}
