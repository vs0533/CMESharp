using CME.Data;
using CME.Data.Repositories;
using CME.Framework.Extentsion;
using CME.Framework.Model;
using CME.Framework.Runtime;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.DynamicLinq;
using System.Linq.Dynamic.Core;
using Newtonsoft.Json;
using System.Collections;
using System.Diagnostics;

namespace CME.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private readonly ServiceCollection _serviceCollection;
        private readonly DbContextOptions<CMEDBContext> _options;
        public IConfigurationRoot Configuration { get; }
        public UnitTest1()
        {
            //var optionBuilder = new DbContextOptionsBuilder<CMEDBContext>();
            //optionBuilder.UseInMemoryDatabase();
            //_options = optionBuilder.Options;
            var jsonpath = System.IO.Directory.GetParent(AppContext.BaseDirectory).Parent.Parent;
            var builder = new ConfigurationBuilder()
                .SetBasePath(jsonpath.FullName)
                .AddJsonFile("entity.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            _serviceCollection = new ServiceCollection();
            _serviceCollection
                .AddEntityFramework()
                .AddDbContext<CMEDBContext>(
                    options =>
                    {
                        options.UseSqlServer("server=.;uid=sa;pwd=Abc@123;database=CMESharp");
                    }
                );

            _serviceCollection.Configure<EntityModelConfig>(
                Configuration.GetSection("EntityModelMetaConfig")
                );
            _serviceCollection.AddSingleton<IModelProvider, DefaultModelProvider>();
            _serviceCollection.AddScoped<IDynamicEntityRepository, DynamicEntityRepository>();
            //optionBuilder.
        }
        [TestMethod]
        public void TestMethod1()
        {
            IServiceProvider provider = _serviceCollection.BuildServiceProvider();
            var modelprovider = provider.GetService<IModelProvider>();
            var types = modelprovider.GetTypes();
            var repository = provider.GetService<IDynamicEntityRepository>();

            using (var ctx = provider.GetService<CMEDBContext>())
            {
                var u = types.Instan("Unit");
                u["UnitName"] = "唐林的单位";
                var uid = ctx.Add(u).Entity["Id"];


                var s = types.Instan("User");
                s["UserCode"] = "tanglin";
                s["Name"] = "唐林";
                s["PassWord"] = "123123";
                s["UnitId"] = uid;

                ctx.Add(s);
                ctx.SaveChanges();

                var type = modelprovider.GetType("User");
                var type1 = modelprovider.GetType("Unit");
            }

        }
        [TestMethod]
        public void TestProperty()
        {
            IServiceProvider provider = _serviceCollection.BuildServiceProvider();
            var modelprovider = provider.GetService<IModelProvider>();

            var typeUser = modelprovider.GetType("User");
            var typeUnit = modelprovider.GetType("Unit");

            using (var ctx = provider.GetService<CMEDBContext>())
            {
                var userq =
                   ctx.GetType()
                   .GetTypeInfo()
                   .GetMethod("Set").MakeGenericMethod(typeUser).Invoke(ctx, null) as IQueryable;

                var unitq = ctx.GetType()
                    .GetTypeInfo()
                    .GetMethod("Set").MakeGenericMethod(typeUnit).Invoke(ctx, null) as IQueryable;

                var s = userq.Where("UserCode=\"tanglin\" and Password = \"123123\"")
                    .ToDynamicList();
                foreach (var item in s)
                {
                    Assert.AreEqual((string)item["UserCode"], "tanglin");
                }
                var q2 = unitq.Join(userq, "it.Id", "UnitId", "new(outer.UnitName as UnitName,inner.Name as User)")
                    .Where("UnitName=\"唐林的单位\"");
                var r21 = q2.ToDynamicArray();
                var r22 = q2.ToDynamicArray<DynamicClass>();


                foreach (var item in r21)
                {
                    Assert.AreEqual((string)item["UnitName"], "唐林的单位");
                }

                var q3 = unitq.Single("UnitName=\"唐林的单位\"");
                var r3 = q3 as DynamicEntity;
                r3["UnitName"] = "我次奥我i你";
                ctx.Update(r3);
                ctx.SaveChanges();
            }
        }
        
    }
}
