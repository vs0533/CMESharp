using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CME.Framework.Runtime;
using System.Reflection;
using System.Reflection.Emit;
using CME.Data;
using CME.Framework.Model;

namespace CME.Api.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IModelProvider provider;
        private readonly CMEDBContext ctx;
        public ValuesController(IModelProvider provider, CMEDBContext ctx)
        {
            this.provider = provider;
            this.ctx = ctx;
        }
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            Type[] s= provider.GetTypes();

            DynamicEntity b =(DynamicEntity) Activator.CreateInstance(s[0]);
            //b["id"] = Guid.NewGuid();
            b["UserCode"] = "tanglin";
            b["Name"] = "唐林";
            b["PassWord"] = "123123";
            b["UnitId"] = Guid.NewGuid();
            ctx.Add(b);
            ctx.SaveChanges();

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
