using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;

namespace CME.Data.Infrastructure
{
    public class RepositoryAsync<T> : RepositoryBase<T> where T : class
    {
        public RepositoryAsync(CMEDBContext dataContext) : base(dataContext)
        {
            
        }

        public virtual async Task AddAsync(T entity)
        {
            await dbset.AddAsync(entity);
        }
        public virtual async Task<T> GetByIdAsync(long id)
        {
            return await dbset.FindAsync(id);
        }
        public virtual async Task<T> GetByIdAsync(string id)
        {
            return await dbset.FindAsync(id);
        }
        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await dbset.FindAsync(id);
        }
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await dbset.ToListAsync();
        }
        public virtual async Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> where)
        {
            return await dbset.Where(where).ToListAsync();
        }
        public async Task<T> GetAsync(Expression<Func<T, bool>> where)
        {
            return await dbset.Where(where).FirstOrDefaultAsync<T>();
        }
    }
}
