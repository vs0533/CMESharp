using CME.Framework.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CME.Framework.Data
{
    /// <summary>
    /// 实体配置元数据 - 实体类
    /// </summary>
    [Table("system_EntityMeta")]
    public class EntityMeta {
        public Guid Id { get; set; }
        [Required,MaxLength(50)]
        public string EntityName { get; set; }
        [Required,MaxLength(50)]
        public string ClassName { get; set; }
        public Guid EntityMetaGroupId { get; set; }
        [ForeignKey("EntityMetaGroupId")]
        public EntityMetaGroup EntityMetaGroup { get; set; }
        public IEnumerable<EntityPropertyMeta> Properties { get; set; }
    }
    /// <summary>
    /// 实体配置元数据 - 实体类的属性
    /// </summary>
    [Table("system_EntityPropertyMeta")]
    public class EntityPropertyMeta {
        public Guid Id { get; set; }
        [Required,MaxLength(50)]
        public string Name { get; set; }
        [Required,MaxLength(50)]
        public string PropertyName { get; set; }
        [Required]
        public int Length { get; set; }
        public bool IsRequired { get; set; }
        [Required,MaxLength(50)]
        public string ValueType { get; set; }
        public string Foreign { get; set; }
        public Guid EntityMetaId { get; set; }
        [ForeignKey("EntityMetaId")]
        public EntityMeta EntityMeta { get; set; }
    }
    /// <summary>
    /// 实体配置元数据 - 实体类分组
    /// </summary>
    [Table("system_EntityMetaGroup")]
    public class EntityMetaGroup {
        public Guid Id { get; set; }
        [Required,MaxLength(50)]
        public string Name { get; set; }
    }
}
