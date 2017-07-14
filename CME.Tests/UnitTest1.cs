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

            var u = types.Instan("Unit");
            u["UnitName"] = "唐林的单位";

            var s = types.Instan("User");
            s["UserCode"] = "tanglin";
            s["Name"] = "唐林";
            s["PassWord"] = "123123";
            s["Unit"] = u;



            repository.Add(s);
            repository.Save();

            using (var context = provider.GetService<CMEDBContext>())
            {

                var type = modelprovider.GetType("User");
                var type1 = modelprovider.GetType("Unit");

               

                var a =
                    context.GetType()
                    .GetTypeInfo()
                    .GetMethod("Set").MakeGenericMethod(type).Invoke(context, null) as IQueryable;

                var t = context.GetType()
                    .GetTypeInfo()
                    .GetMethod("Set").MakeGenericMethod(type1).Invoke(context, null) as IQueryable;

                //var json =JsonConvert.DeserializeObject(r);
            }

            Assert.IsNotNull(s);
            //}

        }
        [TestMethod]
        public void TestProperty()
        {
            IServiceProvider provider = _serviceCollection.BuildServiceProvider();
            var modelprovider = provider.GetService<IModelProvider>();

            var type = modelprovider.GetType("User");

        }
        
    }
}
