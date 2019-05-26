using Grpc.Core;
using System;

namespace DBTogRPCService
{

    internal class Program
    {
        private const int Port = 15000;

        public static void Main(string[] args)
        {
            var repository = new SQLiteRepository();
            repository.DeleteDatabase();

            var server = new Server
            {
                Services = { DBTogRPC.DBTogRPCService.BindService(new DBTogRPCService(repository)) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("Please connect on " + Port);
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
