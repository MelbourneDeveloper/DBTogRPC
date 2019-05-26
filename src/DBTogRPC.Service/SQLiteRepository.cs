using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DBTogRPCService
{
    internal class SQLiteRepository : IRepository
    {
        public async Task<object> GetAsync(Type type, string key)
        {
            //TODO: This code is horrible and synchronous. Refactor to not load all entities in one hit, and make the synchronous

            using (var context = new SQLiteContext())
            {
                var methodInfo = typeof(SQLiteContext).GetMethod(nameof(SQLiteContext.Set));
                var setMethod = methodInfo.MakeGenericMethod(new Type[] { type });

                var dbSet = (IQueryable)setMethod.Invoke(context, new object[] { });

                var includeMethod = typeof(CustomExtensions).GetMethod(nameof(CustomExtensions.Include)).MakeGenericMethod(new Type[] { type });
                var enumerable = (IEnumerable<object>)includeMethod.Invoke(null, new object[] { dbSet, context.GetIncludePaths(type) });
                var entities = enumerable.ToList();

                var properties = type.GetProperties();

                var keyColumn = properties.FirstOrDefault(p =>
                {
                    var customAttributes = p.GetCustomAttributes(typeof(KeyAttribute), true);
                    return customAttributes != null;
                });

                var returnValue = entities.FirstOrDefault(e =>
                {
                    var keyValue = (string)keyColumn.GetValue(e, null);
                    return string.Compare(keyValue, key, true) == 0;
                });

                return returnValue;
            }
        }

        public async Task SaveAsync(object entity)
        {
            using (var db = new SQLiteContext())
            {
                await db.AddAsync(entity);
                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Type type, string key)
        {
            var entity = await GetAsync(type, key);

            using (var db = new SQLiteContext())
            {
                db.Remove(entity);
                await db.SaveChangesAsync();
            }
        }

        public void DeleteDatabase()
        {
            File.Delete("Test.db");
        }
    }
}
