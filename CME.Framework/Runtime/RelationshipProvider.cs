using System;
using System.Collections.Generic;
using System.Text;
using CME.Framework.Model;
using System.Linq;
using Microsoft.Extensions.Options;

namespace CME.Framework.Runtime
{
    public class RelationshipProvider : IRelationshipProvider
    {
        //private readonly IModelProvider modelProvider;
        //private readonly EntityMeta[] meta = null;
        //public RelationshipProvider(IModelProvider modelProvider, IOptions<EntityModelConfig> _config)
        //{
        //    this.modelProvider = modelProvider;
        //    this.meta = _config.Value.Metas;
        //}
        //public IEnumerable<RelationshipInfo> GetRelationship(string typeName)
        //{
        //    RelationshipInfo info = new RelationshipInfo();
        //    var reference = meta.FirstOrDefault(c => c.ClassName == typeName);
        //    info.Reference = reference.ClassName;
        //    var foreign = reference.Properties.FirstOrDefault(c => !string.IsNullOrEmpty(c.Foreign));
        //    info.ForeignKey = foreign.PropertyName;
        //    info.Relationship = foreign.Foreign.Split('.')[0];

        //    return null;
        //}
        public IEnumerable<RelationshipInfo> GetRelationship(string typeName)
        {
            throw new NotImplementedException();
        }
    }
}
