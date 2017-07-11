using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CME.Framework.Model
{
    public static class Extentsion
    {
        
        public static Type GetType(this EntityPropertyMeta meta)
        {
            switch (meta.PropertyName)
            {
                case "string":
                    return  typeof(string);
                case "int":
                    return meta.IsRequired ? typeof(int) : typeof(int?);
                case "datetime":
                    return meta.IsRequired ? typeof(DateTime) : typeof(DateTime?);
                case "bool":
                    return meta.IsRequired ? typeof(bool) : typeof(bool?); 
                default:
                    throw new ArgumentNullException("类型参数无效");
            }
        }
        public static EntityAttribute UseIsRequest(this EntityPropertyMeta meta)
        {
            EntityAttribute ea = new EntityAttribute();
            ea.AttributeType = typeof(RequiredAttribute);
            return ea.AttachErrorMessage(string.Format("必须输入{0}",meta.Name));
        }
        public static EntityAttribute UseStringLength(this EntityPropertyMeta meta)
        {
            EntityAttribute ea = new EntityAttribute();
            ea.AttributeType = typeof(StringLengthAttribute);
            ea.ConstructorArgTypes = new Type[] { typeof(int) };
            ea.ConstructorArgValues = new object[] { meta.Length };
            ea.AttachErrorMessage(string.Format("{0}长度不能超过{1}",meta.Name,meta.Length));
            return ea;
        }
        private static EntityAttribute AttachErrorMessage(this EntityAttribute ea,string msg)
        {
            ea.Properties = new string[] { "ErrorMessage" };
            ea.PropertyValues = new object[] {msg };
            return ea;
        }
        
    }
}
