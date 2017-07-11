using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CME.Framework.Model
{
    public class EntityAttribute
    {
        public Type AttributeType { get; set; }
        public Type[] ConstructorArgTypes { get; set; }
        public object[] ConstructorArgValues { get; set; }
        public string[] Properties { get; set; }
        public object[] PropertyValues { get; set; }
    }
}
