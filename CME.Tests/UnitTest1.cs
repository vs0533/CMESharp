using CME.Data;
using CME.Data.Repositories;
using CME.Framework.Data;
using CME.Framework.Extentsion;
using CME.Framework.Model;
using CME.Framework.Runtime;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;

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

            _serviceCollection.AddDbContext<RuntimeDbContext>(options =>
                options.UseSqlServer("server=.;uid=sa;pwd=Abc@123;database=CMESharp_System")
            );
            _serviceCollection.AddScoped<EntityModelConfigService>();
            _serviceCollection.AddSingleton<IModelProvider, DefaultModelProvider>();
            _serviceCollection.AddScoped<IDynamicEntityRepository, DynamicEntityRepository>();
            //optionBuilder.
        }
        [TestMethod]
        public void TestAddData()
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
        public void TestDynamicReporisoty()
        {
            IServiceProvider provider = _serviceCollection.BuildServiceProvider();
            var modelprovider = provider.GetService<IModelProvider>();

            var dynamicRepository = provider.GetService<IDynamicEntityRepository>();

            var typeUser = modelprovider.GetType("User");
            var query = dynamicRepository.GetQueryByEntity(typeUser);

            var result = query.Where("UserCode = \"tanglin\"").ToDynamicArray();
            var rstr = JsonConvert.SerializeObject(result);
            Assert.IsNotNull(result);
           
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
        [TestMethod]
        public void TestRepositoryDeleteWhere()
        {
            IServiceProvider provider = _serviceCollection.BuildServiceProvider();
            var repository = provider.GetService<IDynamicEntityRepository>();

            //repository.Delete("User","");
        }
        [TestMethod]
        public void TestInitEntityModelConfig()
        {
            IServiceProvider provider = _serviceCollection.BuildServiceProvider();
            var entityModelConfigService = provider.GetService<EntityModelConfigService>();
            using (var ctx = provider.GetService<RuntimeDbContext>())
            {
                ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();
                EntityMetaGroup group = new EntityMetaGroup();
                group.Name = "默认分组";

                EntityMeta meta_User = new EntityMeta();
                meta_User.ClassName = "User";
                meta_User.EntityMetaGroup = group;
                meta_User.EntityName = "单位用户";
                meta_User.Properties = new List<EntityPropertyMeta> {
                    new EntityPropertyMeta{
                        Name = "用户代码",
                        PropertyName = "UserCode",
                        ValueType = "string",
                        IsRequired = true,
                        Length = 50
                    },
                    new EntityPropertyMeta{
                        Name = "用户名",
                        PropertyName = "Name",
                        ValueType = "string",
                        IsRequired = true,
                        Length = 50
                    },
                    new EntityPropertyMeta{
                        Name = "用户密码",
                        PropertyName = "PassWord",
                        ValueType = "string",
                        IsRequired = true,
                        Length = 50
                    },
                    new EntityPropertyMeta{
                        Name = "单位ID",
                        PropertyName = "UnitId",
                        ValueType = "guid",
                        IsRequired = true,
                        Foreign = "Unit.Id"
                    }
                };

                EntityMeta meta_Unit = new EntityMeta();
                meta_Unit.ClassName = "Unit";
                meta_Unit.EntityMetaGroup = group;
                meta_Unit.EntityName = "单位信息";
                meta_Unit.Properties = new List<EntityPropertyMeta> {
                    new EntityPropertyMeta{
                        Name = "单位名称",
                        PropertyName = "Name",
                        ValueType = "string",
                        IsRequired = true,
                        Length = 50
                    },
                    new EntityPropertyMeta{
                        Name = "单位级别",
                        PropertyName = "Level",
                        ValueType = "string",
                        IsRequired = true,
                        Length = 50
                    }
                };

                ctx.Add(meta_User);
                ctx.Add(meta_Unit);
                ctx.SaveChanges();
                
            }

        }
        [TestMethod]
        public void TestRepositoryAdd()
        {
            IServiceProvider provider = _serviceCollection.BuildServiceProvider();
            var ctx = provider.GetService<CMEDBContext>();
            
            var repository = provider.GetService<IDynamicEntityRepository>();
            var modelprovider = provider.GetService<IModelProvider>();

            ctx.Database.EnsureDeleted();
            ctx.Database.EnsureCreated();

            var type_User = modelprovider.GetType("User");
            var user = type_User.Instan();

            //string jsondata = "{\"User\":{\"UserCode\":\"tanglin\",\"Name\":\"唐林\",\"PassWord\":\"123123\"} }";

            //JsonConvert.DeserializeObject<DynamicEntity>(jsondata);

            string ucode = "UserCode";

            user[ucode] = "tanglin";
            user["Name"] = "唐林";
            user["PassWord"] = "123123";
            user["CreateTime"] = DateTime.Now;

            repository.Add(user);
            repository.Save();

        }
        [TestMethod]
        public void TestRepositoryQuery()
        {
            IServiceProvider provider = _serviceCollection.BuildServiceProvider();
            var ctx = provider.GetService<CMEDBContext>();

            var repository = provider.GetService<IDynamicEntityRepository>();
            var modelprovider = provider.GetService<IModelProvider>();

            var type_User = modelprovider.GetType("User");
            var user = type_User.Instan();

            var q1 = repository.GetById("User", new Guid("35A62370-27F4-48A2-8FE7-08D4CC310223"));

            var q2 = repository.Get("User", "Id <> \"35A62370-27F4-48A2-8FE7-08D4CC310223\"");
        }

        [TestMethod]
        public void TestRepositoryEdit()
        {
            IServiceProvider provider = _serviceCollection.BuildServiceProvider();
            var ctx = provider.GetService<CMEDBContext>();

            var repository = provider.GetService<IDynamicEntityRepository>();
            var modelprovider = provider.GetService<IModelProvider>();

            var type_User = modelprovider.GetType("User");
            var user = type_User.Instan();

            var q = repository.Get("User",string.Format("Id <> \"{0}\"",Guid.Empty));

            q["Name"] = "更改第一条";

            repository.Update(q);
            repository.Save();
        }
        [TestMethod]
        public void TestRepositoryDelete()
        {
            IServiceProvider provider = _serviceCollection.BuildServiceProvider();
            var ctx = provider.GetService<CMEDBContext>();

            var repository = provider.GetService<IDynamicEntityRepository>();
            var modelprovider = provider.GetService<IModelProvider>();

            var type_User = modelprovider.GetType("User");
            var user = type_User.Instan();

            var q = repository.Get("User", string.Format("Id <> \"{0}\"", Guid.Empty));
            repository.Delete(q);
            repository.Save();
        }
        [TestMethod]
        public void TestRepositoryDelete_Where()
        {
            IServiceProvider provider = _serviceCollection.BuildServiceProvider();
            var ctx = provider.GetService<CMEDBContext>();

            var repository = provider.GetService<IDynamicEntityRepository>();
            var modelprovider = provider.GetService<IModelProvider>();

            var type_User = modelprovider.GetType("User");
            var user = type_User.Instan();

            repository.Delete("User", string.Format("Id <> \"{0}\"", Guid.Empty));
            repository.Save();
        }


    }
}
