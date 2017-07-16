using CME.Framework.Model;
using CME.Framework.Runtime;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using System.Reflection;


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

        public void Delete(string entityName,string where)
        {
            var type = modelprovider.GetType(entityName);
            var query = this.GetQueryByEntity(type).FirstOrDefault(where);
            Delete(query);
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

        public IQueryable GetQueryByEntity(Type entityType)
        {
            var query = this.dataContext.GetType()
                .GetTypeInfo()
                .GetMethod("Set").MakeGenericMethod(entityType)
                .Invoke(this.dataContext, null) as IQueryable;
            return query;
        }
    }
}
