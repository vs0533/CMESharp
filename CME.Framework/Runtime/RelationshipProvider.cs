using System;
using System.Collections.Generic;
using System.Text;
using CME.Framework.Model;
using System.Linq;
using Microsoft.Extensions.Options;
using CME.Framework.Data;
using System.Linq;

namespace CME.Framework.Runtime
{
    public class RelationshipProvider : IRelationshipProvider
    {
        private readonly IModelProvider modelprovider = null;
        private readonly EntityModelConfigService modelconfig = null;

        private readonly IEnumerable<EntityMeta> metas = null;
        public RelationshipProvider(
            IModelProvider modelprovider,
            EntityModelConfigService modelconfig
            )
        {
            this.modelprovider = modelprovider;
            this.modelconfig = modelconfig;

            metas = modelconfig.GetEntityMetas();
        }

        private EntityMeta NewEntityMeta(EntityMeta old)
        {
            EntityMeta newMeta = new EntityMeta();
            newMeta.ClassName = old.ClassName;
            newMeta.EntityMetaGroup = old.EntityMetaGroup;
            newMeta.EntityMetaGroup = old.EntityMetaGroup;
            newMeta.EntityName = old.EntityName;
            newMeta.Id = old.Id;
            newMeta.Properties = old.Properties;
            return newMeta;
        }
        
        public IEnumerable<EntityMeta> GetParents(EntityMeta meta)
        {

            List<EntityMeta> resultmeta = new List<EntityMeta>();

            //基准类型的外键字段
            IEnumerable<string> foregins = meta.Properties.Where(c => !string.IsNullOrEmpty(c.Foreign)).Select(c => c.Foreign);

            //基准类型的外键表名集合
            var foreginTable = foregins.Select(c => c.Split('.')[0]);

            //根据外键表查找类型
            var parentMeta = this.metas.Where(c => foreginTable.Contains(c.ClassName));
            

            foreach (var item in parentMeta)
            {
                resultmeta.AddRange(GetParents(item));
            }

            foreach (var item in parentMeta)
            {
                var newMeta = NewEntityMeta(item);
                newMeta.Childs = meta;
                resultmeta.Add(newMeta);
            }
            return resultmeta;

        }
        public IEnumerable<EntityMeta> GetChilds(EntityMeta meta)
        {
            List<EntityMeta> metaresult = new List<EntityMeta>();

            var childMeta = metas.Where(c => c.Properties.Where(d=> !string.IsNullOrEmpty(d.Foreign)).Where(p=>p.Foreign.Split('.')[0] == meta.ClassName).Count()>0);

            foreach (var item in childMeta)
            {
                metaresult.AddRange(GetChilds(item));
            }

            foreach (var item in childMeta)
            {
                var newMeta = NewEntityMeta(item);
                newMeta.Parent = meta;

                metaresult.Add(newMeta);
            }
            return metaresult;
        }
    }
}
