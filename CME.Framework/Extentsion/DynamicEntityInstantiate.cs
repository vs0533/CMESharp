using CME.Framework.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CME.Framework.Extentsion
{
    public static class DynamicEntityInstantiate
    {
        public static DynamicEntity Instan(this Type type)
        {
            return Activator.CreateInstance(type) as DynamicEntity;
        }
        public static DynamicEntity Instan(this Type[] types,string typeName)
        {
            Type type = types.First(c => c.Name == typeName);
            return Activator.CreateInstance(type) as DynamicEntity;
        }
    }
}
