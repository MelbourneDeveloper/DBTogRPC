using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Threading.Tasks;

namespace DBTogRPC.Client
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Go().Wait();
        }

        private static async Task Go()
        {
            var channel = new Channel("127.0.0.1:15000", ChannelCredentials.Insecure);

            var dbTogRPCServiceClient = new DBTogRPCService.DBTogRPCServiceClient(channel);

            var personKey = new Guid("07b10373-0487-4281-b768-81fdc48c0318");

            var addressKey = new Guid("669c71bf-7c4e-4536-9642-bce10f22b7bd");

            //Save the Person on the server
            var reply = dbTogRPCServiceClient.Save(
                new SaveRequest
                {
                    TypeName = "Person",
                    DTO =
                    Any.Pack
                    (
                        new Person
                        {
                            PersonKey = personKey.ToString(),
                            FirstName = "you",
                            BillingAddress = new Address
                            {
                                AddressKey = addressKey.ToString()
                            }

                        }
                   )
                });

            //Load the Person from the server
            var any = dbTogRPCServiceClient.Get(new DTORequest { TypeName = "Person", KeyValue = personKey.ToString() });
            var person = any.Unpack<Person>();

            Console.WriteLine($"Got Person: {person.PersonKey} Billing Address: {person.BillingAddress.AddressKey}");

            //Load the Address from the server
            any = dbTogRPCServiceClient.Get(new DTORequest { TypeName = "Address", KeyValue = addressKey.ToString() });
            var address = any.Unpack<Address>();

            Console.WriteLine($"Got Address: {address.AddressKey}");

            var secondAddress = new Address { AddressKey = Guid.NewGuid().ToString() };

            //Save a new Address on the server
            await dbTogRPCServiceClient.SaveAsync(new SaveRequest { DTO = Any.Pack(secondAddress), TypeName = "Address" });

            //Delete the person
            var result = await dbTogRPCServiceClient.DeleteAsync(new DTORequest { TypeName = "Person", KeyValue = personKey.ToString() });

            Console.ReadLine();
        }
    }
}
