
using AutoMapper;
using DBTogRPC;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Threading.Tasks;

namespace DBTogRPCService
{
    internal class DBTogRPCService : DBTogRPC.DBTogRPCService.DBTogRPCServiceBase
    {
        #region Fields
        private readonly IMapper _FromEntityMapper;
        private readonly IMapper _ToEntityMapper;
        #endregion

        #region Public Properties
        public IRepository Repository { get; }
        #endregion

        #region Constructor
        public DBTogRPCService(IRepository repository)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Entities.Person, Person>();
                cfg.CreateMap<Entities.Address, Person>();
            });
            _FromEntityMapper = config.CreateMapper();

            config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Person, Entities.Person>();
                cfg.CreateMap<Address, Entities.Address>();
            });
            _ToEntityMapper = config.CreateMapper();

            Repository = repository;
        }
        #endregion

        #region Implementation
        public override async Task<Any> Get(DTORequest request, ServerCallContext context)
        {
            var entityType = GetEntityType(request.TypeName);

            var dtoType = GetDTOType(request.TypeName);

            var entity = await Repository.GetAsync(entityType, request.KeyValue);

            var message = (IMessage)_FromEntityMapper.Map(entity, entityType, dtoType);

            Console.WriteLine($"1 {request.TypeName} returned with a key of {request.KeyValue}.");

            return Any.Pack(message);
        }

        // Server side handler of the Add RPC
        public override async Task<Result> Save(SaveRequest request, ServerCallContext context)
        {
            var entityType = GetEntityType(request.TypeName);

            var dtoType = GetDTOType(request.TypeName);

            var method = typeof(Any).GetMethod(nameof(Any.Unpack)).MakeGenericMethod(new System.Type[] { dtoType });

            var dto = method.Invoke(request.DTO, null);

            var mappedEntity = _ToEntityMapper.Map(dto, dtoType, entityType);

            await Repository.SaveAsync(mappedEntity);

            Console.WriteLine($"1 {request.TypeName} saved.");

            return new Result();
        }

        public override async Task<Result> Delete(DTORequest request, ServerCallContext context)
        {
            var entityType = GetEntityType(request.TypeName);
            await Repository.DeleteAsync(entityType, request.KeyValue);

            Console.WriteLine($"1 {request.TypeName} deleted with a key of {request.KeyValue}.");

            return new Result();
        }
        #endregion

        #region Helpers

        private static System.Type GetEntityType(string name)
        {
            return System.Type.GetType($"Entities.{name}");
        }

        private static System.Type GetDTOType(string name)
        {
            return typeof(Person).Assembly.GetType($"DBTogRPC.{name}");

        }
        #endregion
    }
}
