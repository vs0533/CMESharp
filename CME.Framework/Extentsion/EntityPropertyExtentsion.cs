using CME.Framework.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CME.Framework.Data;

namespace CME.Framework.Extentsion
{
    public static class EntityPropertyExtentsion
    {
        
        public static void UseType(this EntityProperty ep, EntityPropertyMeta meta)
        {
            switch (meta.ValueType)
            {
                case "string":
                    ep.PropertyType = typeof(string);
                    break;
                case "int":
                    ep.PropertyType = meta.IsRequired ? typeof(int) : typeof(int?);
                    break;
                case "datetime":
                    ep.PropertyType = meta.IsRequired ? typeof(DateTime) : typeof(DateTime?);
                    break;
                case "bool":
                    ep.PropertyType = meta.IsRequired ? typeof(bool) : typeof(bool?);
                    break;
                case "guid":
                    ep.PropertyType = meta.IsRequired ? typeof(Guid) : typeof(Guid?);
                    break;
                default:
                    throw new ArgumentNullException("类型参数无效");
            }
        }
        public static void UseIsRequest(this EntityProperty ep, EntityPropertyMeta meta)
        {
            if (meta.IsRequired)
            {
                EntityAttribute ea = new EntityAttribute();
                ea.AttributeType = typeof(RequiredAttribute);
                ea.AttachErrorMessage(string.Format("必须输入{0}", meta.Name));
                ep.Attributes.Add(ea);
            }
            
        }
        public static void UseStringLength(this EntityProperty ep, EntityPropertyMeta meta)
        {
            if (meta.Length > 0)
            {
                EntityAttribute ea = new EntityAttribute();
                ea.AttributeType = typeof(StringLengthAttribute);
                ea.ConstructorArgTypes = new Type[] { typeof(int) };
                ea.ConstructorArgValues = new object[] { meta.Length };
                ea.AttachErrorMessage(string.Format("{0}长度不能超过{1}", meta.Name, meta.Length));
                ep.Attributes.Add(ea);
            }
            
        }
        private static EntityAttribute AttachErrorMessage(this EntityAttribute ea,string msg)
        {
            ea.Properties = new string[] { "ErrorMessage" };
            ea.PropertyValues = new object[] {msg };
            return ea;
        }
        
    }
}
