using CME.Framework.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CME.Framework.Data
{
    public class EntityModelConfigService
    {
        private readonly RuntimeDbContext dbContext;
        private readonly IMemoryCache _cache;
        private static string cacheKey = "ModelConfigCache";
        public EntityModelConfigService(RuntimeDbContext dbContext, IMemoryCache cache)
        {
            this.dbContext = dbContext;
            this._cache = cache;
        }

        public IEnumerable<EntityMeta> GetEntityMetas()
        {
            IEnumerable<EntityMeta> result = _cache.GetOrCreate(
                    cacheKey,
                    entity=>
                    {
                        return dbContext.EntityMetas
                            .Include(c => c.Properties)
                            .Include(c => c.EntityMetaGroup).ToList();
                    }
                );
            return result;
        }
    }
}
