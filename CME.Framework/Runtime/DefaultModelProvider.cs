using CME.Framework.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CME.Framework.Runtime
{
    public class DefaultModelProvider:IModelProvider
    {
        private Dictionary<Guid, Type> _resultMap = null;
        private readonly IOptions<EntityModelConfig> _config = null;
        private object _lock = new object();
        public DefaultModelProvider(IOptions<EntityModelConfig> config)
        {
            this._config = config;
        }

        public Dictionary<Guid,Type> Map {
            get {
                if (_resultMap == null)
                {
                    lock (_lock)
                    {
                        _resultMap = new Dictionary<Guid, Type>();
                        foreach (var item in _config.Value.Metas)
                        {
                            var result = RuntimeBuilder.Builder(GetEntityFromMeta(item),true);
                            _resultMap.Add(item.EntityId, result);
                        }
                    }
                }
                return _resultMap;
            }
        }
        public Type GetType(Guid modelId)
        {
            Dictionary<Guid, Type> map = Map;
            Type result = null;
            if (!map.TryGetValue(modelId,out result))
            {
                throw new NotSupportedException("没有找到指定ID的模型");
            }
            return result;
        }
        public Type[] GetTypes() {
            Guid[] entityids = _config.Value.Metas.Select(c => c.EntityId).ToArray();
            return Map.Where(c => entityids.Contains(c.Key)).Select(c => c.Value).ToArray();
        }

        private Entity GetEntityFromMeta(EntityMeta meta)
        {
            Entity entity = new Entity();
            entity.BaseType = typeof(DynamicEntity);
            entity.TypeName = meta.ClassName;
            foreach (var item in meta.Properties)
            {
                EntityProperty ep = new EntityProperty();
                ep.PropertyName = item.PropertyName;
                
                ep.UseIsRequest(item);
                ep.UseStringLength(item);
                ep.UseType(item);

                entity.Propertys.Add(ep);
            }
            return entity;
        }
    }
}
