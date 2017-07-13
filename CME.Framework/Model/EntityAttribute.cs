using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CME.Framework.Model
{
    /// <summary>
    /// 标识一个动态类型的 特性标注
    /// </summary>
    public class EntityAttribute
    {
        /// <summary>
        /// 标注的类型
        /// </summary>
        public Type AttributeType { get; set; }
        /// <summary>
        /// 标注类型的构造函数参数类型列表
        /// </summary>
        public Type[] ConstructorArgTypes { get; set; }
        /// <summary>
        /// 标注类型的构造函数参数值列表
        /// </summary>
        public object[] ConstructorArgValues { get; set; }
        /// <summary>
        /// 标注类型的属性名称
        /// </summary>
        public string[] Properties { get; set; }
        /// <summary>
        /// 标注类型的属性值
        /// </summary>
        public object[] PropertyValues { get; set; }
    }
}
