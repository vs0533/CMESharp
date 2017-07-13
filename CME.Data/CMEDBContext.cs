using CME.Framework.Runtime;
using Microsoft.EntityFrameworkCore;
using System;

namespace CME.Data
{
    public class CMEDBContext:DbContext
    {
        private readonly IModelProvider modelprovider;
        public CMEDBContext(DbContextOptions<CMEDBContext> options, IModelProvider modelprovider) :base(options)
        {
            this.modelprovider = modelprovider;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Type[] types = modelprovider.GetTypes();
            foreach (var item in types)
            {
                modelBuilder.Model.AddEntityType(item);
            }
            base.OnModelCreating(modelBuilder);
        }
    }
}
