﻿using EnvDT.Model.IRepository;
using Microsoft.EntityFrameworkCore;
using System;

namespace EnvDT.DataAccess.Repository
{
    public class GenericRepository<TEntity,TContext> : IGenericRepository<TEntity>
        where TEntity:class
        where TContext:DbContext
    {
        protected readonly TContext Context;

        protected GenericRepository(TContext context)
        {
            this.Context = context;
        }

        public void Create(TEntity model)
        {
            Context.Set<TEntity>().Add(model);
        }

        public void Delete(TEntity model)
        {
            Context.Set<TEntity>().Remove(model);
        }

        public virtual TEntity GetById(Guid id)
        {
            return Context.Set<TEntity>().Find(id);
        }

        public virtual TEntity GetFirst()
        {
            return Context.Set<TEntity>().Find();
        }

        public bool HasChanges()
        {
            return Context.ChangeTracker.HasChanges();
        }

        public void Save()
        {
            Context.SaveChanges();
        }
    }
}