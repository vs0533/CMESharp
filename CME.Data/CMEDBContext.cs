using CME.Framework.Runtime;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace CME.Data
{
    public class CMEDBContext:DbContext
    {
        private readonly ICoreConventionSetBuilder _builder;
        private readonly IModelProvider _modelprovider;
        private readonly IMemoryCache _cache;
        private static string DynamicCacheKey = "DynamicModel";
        public CMEDBContext(
            DbContextOptions<CMEDBContext> options, 
            IModelProvider modelprovider,
            ICoreConventionSetBuilder builder,
            IMemoryCache cache
        ) :base(options)
        {
            this._modelprovider = modelprovider;
            this._builder = builder;
            this._cache = cache;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IMutableModel model = _cache.GetOrCreate(
                DynamicCacheKey,
                entry=>
                {
                    ModelBuilder mbuilder = new ModelBuilder(_builder.CreateConventionSet());
                    Type[] types = _modelprovider.GetTypes();
                    foreach (var item in types)
                    {
                        mbuilder.Model.AddEntityType(item);
                    }
                    _cache.Set(DynamicCacheKey, mbuilder.Model);
                    return mbuilder.Model;
                }
            );
            base.OnConfiguring(optionsBuilder);
        }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    Type[] types = _modelprovider.GetTypes();
        //    foreach (var item in types)
        //    {
        //        modelBuilder.Model.AddEntityType(item);
        //    }
        //    base.OnModelCreating(modelBuilder);
        //}
    }
}
