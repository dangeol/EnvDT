using System;
using System.Collections.Generic;

namespace EnvDT.Model.IRepository
{
    public interface IGenericRepository<T>
    {
        T GetById(Guid? id);
        T GetFirst();
        IEnumerable<T> GetAll();
        void Create(T model);
        void Delete(T model);
        bool HasChanges();
    }
}