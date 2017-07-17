using CME.Framework.Data;
using CME.Framework.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CME.Framework.Runtime
{
    public interface IRelationshipProvider
    {
        IEnumerable<EntityMeta> GetParents(EntityMeta meta);
        IEnumerable<EntityMeta> GetChilds(EntityMeta meta);
    }
}
