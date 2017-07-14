using CME.Framework.Model;
using System;
using System.Collections.Generic;
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
        void Delete(Expression<Func<DynamicEntity, bool>> where);
        DynamicEntity GetById(Guid Id);
        DynamicEntity Get(Expression<Func<DynamicEntity, bool>> where);
        IEnumerable<DynamicEntity> GetAll();
        IEnumerable<DynamicEntity> GetMany(Expression<Func<DynamicEntity, bool>> where);
    }
}
