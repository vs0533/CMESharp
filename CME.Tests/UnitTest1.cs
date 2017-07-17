using CME.Data;
using CME.Data.Repositories;
using CME.Framework.Data;
using CME.Framework.Extentsion;
using CME.Framework.Model;
using CME.Framework.Runtime;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
            _serviceCollection.AddScoped<IModelProvider, DefaultModelProvider>();
            _serviceCollection.AddScoped<IRelationshipProvider, RelationshipProvider>();
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
                group.Name = "单位管理";



                EntityMeta meta_User = new EntityMeta();
                meta_User.ClassName = "Student";
                meta_User.EntityMetaGroup = group;
                meta_User.EntityName = "学员账号";
                meta_User.Properties = new List<EntityPropertyMeta> {
                    new EntityPropertyMeta{
                        Name = "账号代码",
                        PropertyName = "UserCode",
                        ValueType = "string",
                        IsRequired = true,
                        Length = 50
                    },
                    new EntityPropertyMeta{
                        Name = "姓名",
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
                    },
                    new EntityPropertyMeta{
                        Name = "科目ID",
                        PropertyName = "ExamSubjectID",
                        ValueType = "guid",
                        Foreign = "ExamSubject.Id",
                        IsRequired = true
                    }
                };

                EntityMeta meta_Unitcategory = new EntityMeta();
                meta_Unitcategory.ClassName = "UnitCategory";
                meta_Unitcategory.EntityMetaGroup = group;
                meta_Unitcategory.EntityName = "单位类型";
                meta_Unitcategory.Properties = new List<EntityPropertyMeta> {
                    new EntityPropertyMeta{
                        Name = "类型名称",
                        PropertyName = "Name",
                        ValueType = "string",
                        IsRequired = true,
                        Length = 50
                    }
                };
                EntityMeta meta_level = new EntityMeta();
                meta_level.ClassName = "UnitLevel";
                meta_level.EntityMetaGroup = group;
                meta_level.EntityName = "单位级别";
                meta_level.Properties = new List<EntityPropertyMeta> {
                    new EntityPropertyMeta{
                        Name = "级别名称",
                        PropertyName = "Name",
                        ValueType = "string",
                        IsRequired = true,
                        Length = 50
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
                        Name = "单位类型ID",
                        PropertyName = "UnitCategory",
                        ValueType = "guid",
                        Foreign="UnitCategory.Id",
                        IsRequired = true
                    },
                    new EntityPropertyMeta{
                        Name = "单位级别ID",
                        PropertyName = "UnitLevelId",
                        ValueType = "guid",
                        Foreign="UnitLevel.Id",
                        IsRequired = true
                    },
                };

                EntityMeta meta_examsubject = new EntityMeta();
                meta_examsubject.ClassName = "ExamSubject";
                meta_examsubject.EntityMetaGroup = group;
                meta_examsubject.EntityName = "科目信息";
                meta_examsubject.Properties = new List<EntityPropertyMeta>
                {
                    new EntityPropertyMeta{
                        Name = "科目名称",
                        PropertyName = "Name",
                        ValueType = "string",
                        IsRequired = true,
                        Length = 50
                    }
                };

                EntityMeta meta_apply = new EntityMeta();
                meta_apply.ClassName = "Apply";
                meta_apply.EntityMetaGroup = group;
                meta_apply.EntityName = "学员报名";
                meta_apply.Properties = new List<EntityPropertyMeta>
                {
                    new EntityPropertyMeta{
                        Name = "科目ID",
                        PropertyName = "ExamSubjectID",
                        ValueType = "guid",
                        Foreign = "ExamSubject.Id",
                        IsRequired = true
                    },
                    new EntityPropertyMeta{
                        Name = "学员ID",
                        PropertyName = "StudentID",
                        ValueType = "guid",
                        Foreign = "Student.Id",
                        IsRequired = true
                    }
                };

                EntityMeta meta_examresult = new EntityMeta();
                meta_examresult.ClassName = "ExamResult";
                meta_examresult.EntityMetaGroup = group;
                meta_examresult.EntityName = "考试成绩";
                meta_examresult.Properties = new List<EntityPropertyMeta>
                {
                    new EntityPropertyMeta{
                        Name = "科目ID",
                        PropertyName = "ExamSubjectID",
                        ValueType = "guid",
                        Foreign = "ExamSubject.Id",
                        IsRequired = true
                    },
                    new EntityPropertyMeta{
                        Name = "学员ID",
                        PropertyName = "StudentID",
                        ValueType = "guid",
                        Foreign = "Student.Id",
                        IsRequired = true
                    },
                    //new EntityPropertyMeta{
                    //    Name = "报名信息",
                    //    PropertyName = "ApplyID",
                    //    ValueType = "guid",
                    //    Foreign = "Apply.Id",
                    //    IsRequired = true
                    //},
                    new EntityPropertyMeta{
                        Name = "分数",
                        PropertyName = "Score",
                        ValueType = "int",
                        IsRequired = true
                    }
                };

                ctx.Add(group);
                ctx.Add(meta_Unitcategory);
                ctx.Add(meta_level);
                ctx.Add(meta_User);
                ctx.Add(meta_Unit);
                ctx.Add(meta_examsubject);
                ctx.Add(meta_apply);
                ctx.Add(meta_examresult);
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
        [TestMethod]
        public void TestInMemoryCache()
        {
            IServiceProvider provider = _serviceCollection.BuildServiceProvider();

            IMemoryCache cache = provider.GetService<IMemoryCache>();
            var modelprovider = provider.GetService<IModelProvider>();
            //var q = modelprovider.GetTypes();

            //var q1 = modelprovider.GetTypes();
            //cache.Remove("TypCache");
            //var q2 = modelprovider.GetTypes();
            modelprovider.ClearCacheReLoad();
        }

        [TestMethod]
        public void TestRelationship_Parent()
        {
            IServiceProvider provider = _serviceCollection.BuildServiceProvider();
            var relationship = provider.GetService<IRelationshipProvider>();
            var modelconfig = provider.GetService<EntityModelConfigService>();

            var userconfig = modelconfig.GetEntityMetas().FirstOrDefault(c => c.ClassName == "Apply");

           var r = relationship.GetParents(userconfig);

            StringBuilder sb = new StringBuilder();
            //foreach (var item in r)
            //{

            //    var result = (string.Format("{0}:{1}",
            //        item.ClassName,
            //        string.Join("->", item.Select(c => c.ClassName))
            //        ));
            //    Console.WriteLine(result);
            //    Debug.WriteLine(result);
            //    sb.Append(result);
            //}
        }
        [TestMethod]
        public void TestRelationship_Child()
        {
            IServiceProvider provider = _serviceCollection.BuildServiceProvider();
            var relationship = provider.GetService<IRelationshipProvider>();
            var modelconfig = provider.GetService<EntityModelConfigService>();

            var userconfig = modelconfig.GetEntityMetas().FirstOrDefault(c => c.ClassName == "Unit");

            var s = relationship.GetChilds(userconfig);
        }
    }
}
