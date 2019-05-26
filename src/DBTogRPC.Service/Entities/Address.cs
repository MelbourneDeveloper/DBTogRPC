using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Address
    {
        [Key]
        public string AddressKey { get; set; }
        public string StreeNumber { get; set; }
        public string Street { get; set; }
        public string Suburb { get; set; }
    }
}
