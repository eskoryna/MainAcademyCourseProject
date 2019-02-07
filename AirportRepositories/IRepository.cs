using System;
using System.Collections.Generic;

namespace DAL
{
    public interface IRepository<T>
    {
        int Add(T entity);
        int AddRange(IList<T> entities);
        int Delete(T entity);
        int Save(T entity);
        // T GetOne(int? id);
        // List<T> GetAll();
    }
}
