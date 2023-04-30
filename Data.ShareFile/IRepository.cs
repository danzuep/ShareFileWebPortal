using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Data.ShareFile
{
    public interface IRepository<T>
    {
        Task<IList<T>> GetAll();
        Task<T> Get(int PrimaryKey);
        Task<IList<T>> Get<T2>(Expression<Func<T, bool>> predicate, Expression<Func<T, T2>> order);
        Task<int> Save(T Item);
        Task<int> Save(IEnumerable<T> Items);
        Task<int> Delete(int PrimaryKey);
        Task<int> Delete(IEnumerable<int> PrimaryKeys);
        //Task<int> SaveChanges();
    }
}
