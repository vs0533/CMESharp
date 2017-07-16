using CME.Framework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace CME.Data.Infrastructure
{
    public interface IRepositoryDynamic
    {
        void Save();
        void Add(DynamicEntity entity);
        void Update(DynamicEntity entity);
        void Delete(DynamicEntity entity);
        void Delete(string entityName, string where);
        DynamicEntity GetById(string typeName,Guid Id);
        DynamicEntity Get(string typeName,string where);
        IEnumerable<DynamicEntity> GetAll(string typeName);
        IEnumerable<DynamicEntity> GetMany(string typeName,string where);

        IQueryable GetQueryByEntity(Type entityType);
    }
}
