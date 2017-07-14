using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using CME.Framework.Model;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using CME.Framework.Runtime;

namespace CME.Data.Infrastructure
{
    public abstract class RepositoryDynamic : IRepositoryDynamic
    {
        private readonly CMEDBContext dataContext;
        private readonly DbSet<DynamicEntity> dbSet;
        private readonly IModelProvider modelprovider;
        public RepositoryDynamic(CMEDBContext dataContext,IModelProvider modelprovider)
        {
            this.modelprovider = modelprovider;
            this.dataContext = dataContext;
        }
        public IDbContextTransaction BeginTransaction()
        {
            var train = dataContext.Database.CurrentTransaction;
            if (train == null)
            {
                train = dataContext.Database.BeginTransaction();
            }
            return train;
        }
        public void Save()
        {
            dataContext.SaveChanges();
        }
        public void Commit()
        {
            BeginTransaction().Commit();
        }
        public void Rollback()
        {
            BeginTransaction().Rollback();
        }
        public void Add(DynamicEntity entity)
        {
            dataContext.Add(entity);
        }

        public void Delete(DynamicEntity entity)
        {
            dataContext.Remove(entity);
        }

        public void Delete(Expression<Func<DynamicEntity, bool>> where)
        {
            throw new NotImplementedException();
        }

        public DynamicEntity Get(Expression<Func<DynamicEntity, bool>> where)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DynamicEntity> GetAll()
        {
            throw new NotImplementedException();
        }

        public DynamicEntity GetById(Guid Id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DynamicEntity> GetMany(Expression<Func<DynamicEntity, bool>> where)
        {
            throw new NotImplementedException();
        }

        

        public void Update(DynamicEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
