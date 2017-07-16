using CME.Framework.Model;
using CME.Framework.Extentsion;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using CME.Framework.Data;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CME.Framework.Runtime
{
    public class DefaultModelProvider:IModelProvider
    {
        private Dictionary<Guid, Type> _resultMap = null;
        private readonly EntityModelConfigService _modelConfigService = null;
        private readonly IEnumerable<Data.EntityMeta> _entityMeta = null;
        private readonly IMemoryCache _cache = null;
        private static string typeCacheKey = "TypCache";
        private object _lock = new object();
        public DefaultModelProvider(EntityModelConfigService _modelConfigService,IMemoryCache cache)
        {
            this._modelConfigService = _modelConfigService;
            this._entityMeta = _modelConfigService.GetEntityMetas();
            this._cache = cache;
        }

        public Dictionary<Guid,Type> Map {
            get {
                _resultMap = _cache.GetOrCreate(
                    typeCacheKey,
                    resultmap =>
                    {
                        _resultMap = new Dictionary<Guid, Type>();
                        foreach (var item in _modelConfigService.GetEntityMetas())
                        {
                            var result = RuntimeBuilder.Builder(GetEntityFromMeta(item), true);
                            _resultMap.Add(item.Id, result);
                        }
                        return _resultMap;
                    }
                );
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
        public Type GetType(string typeName)
        {
            Dictionary<Guid, Type> map = Map;
            var result = map.FirstOrDefault(c => c.Value.Name == typeName).Value;
            if (result == null)
            {
                throw new NotSupportedException("没有找到指定typeName的模型");
            }
            return result;
        }
        public Type[] GetTypes() {
            Guid[] entityids = _entityMeta.Select(c => c.Id).ToArray();
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
        public void ClearCacheReLoad()
        {
            var typecache = _cache.Get<Dictionary<Guid, Type>>("typeCacheKey");
            var modelcache = _cache.Get<IMutableModel>("DynamicModel");
            var modelconfig = _cache.Get<IEnumerable<EntityMeta>>("ModelConfigCache");

            if (typecache != null)
            {
                _cache.Remove("typeCacheKey");
            }
            if (modelcache != null)
            {
                _cache.Remove("DynamicModel");
            }
            if (modelconfig != null)
            {
                _cache.Remove("ModelConfigCache");
            }

        }
    }
}
