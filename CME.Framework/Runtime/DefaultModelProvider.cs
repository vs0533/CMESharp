using CME.Framework.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CME.Framework.Runtime
{
    public class DefaultModelProvider
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
                            _resultMap.Add(item.EntityId, typeof(object));
                        }
                    }
                }
                return _resultMap;
            }
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
                
                ep.Attributes.Add(item.UseIsRequest());
                ep.Attributes.Add(item.UseStringLength());
                ep.PropertyType = item.GetType();

                entity.Propertys.Add(ep);
            }
            return entity;
        }
    }
}
