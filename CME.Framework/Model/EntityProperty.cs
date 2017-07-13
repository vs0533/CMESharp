using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CME.Framework.Model
{
    /// <summary>
    /// 标识一个实体类型的 属性配置
    /// </summary>
    public class EntityProperty
    {
        /// <summary>
        /// 属性的类型
        /// </summary>
        public Type PropertyType{ get; set; }
        /// <summary>
        /// 属性的名称
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// 属性的特性标注列表
        /// </summary>
        public List<EntityAttribute> Attributes { get; set; }
        public EntityProperty()
        {
            this.Attributes = new List<EntityAttribute>();
        }
    }
}
