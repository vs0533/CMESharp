using CME.Framework.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CME.Framework.Extentsion
{
    public static class RelationshipExtentsion
    {
        public static Dictionary<EntityMeta, IEnumerable<EntityMeta>> Parents(this EntityMeta meta)
        {
            Dictionary<EntityMeta, IEnumerable<EntityMeta>> result = new Dictionary<EntityMeta, IEnumerable<EntityMeta>>();

            var Foregins = meta.Properties.Select(c => c.Foreign.Split('.')[0]);

            throw new NotImplementedException();
        }
    }
}
