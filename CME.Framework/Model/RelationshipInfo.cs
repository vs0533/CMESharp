using CME.Framework.Data;
using CME.Framework.Enumeration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CME.Framework.Model
{
    public class RelationshipInfo
    {
        public List<EntityMeta> Parents { get; set; }
        public int Level { get; set; }
    }
}