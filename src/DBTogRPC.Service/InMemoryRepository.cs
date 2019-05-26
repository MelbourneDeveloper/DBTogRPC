using Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DBTogRPCService
{
    public class InMemoryRepository : IRepository
    {
        public List<Address> Addresses { get; } = new List<Address>();
        public List<Person> People { get; } = new List<Person>();

        public async Task DeleteAsync(Type type, string key)
        {
        }

        public async Task<object> GetAsync(Type entityType, string key)
        {
            return new Person { PersonKey = Guid.NewGuid().ToString(), FirstName="Test", Surname="Test", BillingAddress= 
                new Address
                {
                    AddressKey =Guid.NewGuid().ToString(),
                     StreeNumber="10",
                      Street="Test St",
                       Suburb="Somewhere"
                }
            };
        }

        public async Task SaveAsync(object entity)
        {
        }
    }
}
