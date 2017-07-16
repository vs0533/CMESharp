using CME.Framework.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CME.Framework.Data
{
    public class EntityModelConfigService
    {
        private readonly RuntimeDbContext dbContext;
        public EntityModelConfigService(RuntimeDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IEnumerable<EntityMeta> GetEntityMetas()
        {
            return dbContext.EntityMetas
                .Include(c => c.Properties)
                .Include(c=>c.EntityMetaGroup).ToList();
        }
    }
}
