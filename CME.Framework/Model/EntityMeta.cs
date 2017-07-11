using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CME.Framework.Model
{
    public class EntityMeta
    {
        public Guid EntityId { get; set; }
        public string EntityName { get; set; }
        public string ClassName { get; set; }

        public EntityPropertyMeta[] Properties { get; set; }
    }
}
