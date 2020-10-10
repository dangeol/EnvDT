using System;

namespace EnvDT.Model.IRepository
{
    public interface IGenericRepository<T>
    {
        T GetById(Guid id);
        T GetFirst();
        void Create(T model);
        void Delete(T model);
        bool HasChanges();
    }
}