using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CME.Framework.Data
{
    public class RuntimeDbContext:DbContext
    {
        public DbSet<EntityMeta> EntityMetas { get; set; }
        public DbSet<EntityPropertyMeta> EntityPropertyMetas { get; set; }
        public DbSet<EntityMetaGroup> EntityMetaGroups { get; set; }
        public RuntimeDbContext(DbContextOptions<RuntimeDbContext> options) :base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder); 
        }
    }
}
