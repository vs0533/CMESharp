using CME.Framework.Runtime;
using Microsoft.EntityFrameworkCore;
using System;

namespace CME.Api
{
    //public class dbContext:DbContext
    //{
    //    private readonly IModelProvider provider;
    //    public dbContext(DbContextOptions<dbContext> options, IModelProvider provider):base(options)
    //    {
    //        this.provider = provider;
            
    //    }
    //    protected override void OnModelCreating(ModelBuilder modelBuilder)
    //    {
    //        //_modelProvider就是我们上面定义的IRuntimeModelProvider,通过依赖注入方式获取到实例
    //        Type[] runtimeModels = provider.GetTypes();
    //        foreach (var item in runtimeModels)
    //        {
    //            modelBuilder.Model.AddEntityType(item);
    //        }
    //        base.OnModelCreating(modelBuilder);
    //    }
    //}
}
