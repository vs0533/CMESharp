using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CME.Framework.Model
{
    public class Entity
    {
        public Type BaseType { get; set; }
        public string TypeName { get; set; }
        public List<EntityProperty> Propertys{ get; set; }
        public List<EntityAttribute> Attrbutes { get; set; }

        public Entity()
        {
            this.Propertys = new List<EntityProperty>();
            this.Attrbutes = new List<EntityAttribute>();
        }
    }
}
