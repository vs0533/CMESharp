using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CME.Framework.Model
{
    public class EntityProperty
    {
        public Type PropertyType{ get; set; }
        public string PropertyName { get; set; }
        public List<EntityAttribute> Attributes { get; set; }
        public EntityProperty()
        {
            this.Attributes = new List<EntityAttribute>();
        }
    }
}
