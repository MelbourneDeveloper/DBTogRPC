using System;
using System.Threading.Tasks;

namespace DBTogRPCService
{
    public interface IRepository
    {
        Task<object> GetAsync(Type type, string key);
        Task SaveAsync(object entity);
        Task DeleteAsync(Type type, string key);
    }
}
