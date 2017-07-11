using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CME.Framework.Model
{
    public class EntityPropertyMeta
    {
        public string Name { get; set; }
        public string PropertyName { get; set; }
        public int Length { get; set; }
        public bool IsRequired { get; set; }
        public string ValueType { get; set; }
    }
}
