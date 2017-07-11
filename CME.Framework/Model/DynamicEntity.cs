using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CME.Framework.Model
{
    public class DynamicEntity
    {
        private Dictionary<object, object> _attrs;

        public DynamicEntity()
        {
            _attrs = new Dictionary<object, object>();
        }
        public DynamicEntity(Dictionary<object,object> dict)
        {
            _attrs = dict;
        }
        public static DynamicEntity Parse(object obj)
        {
            DynamicEntity model = new DynamicEntity();
            foreach (PropertyInfo item in obj.GetType().GetProperties())
            {
                model._attrs.Add(item.Name, item.GetValue(obj, null));
            }
            return model;
        }
        public T GetValue<T>(string field)
        {
            object obj = null;
            if (!_attrs.TryGetValue(field,out obj))
            {
                _attrs.Add(field, default(T));
            }
            if (obj == null)
            {
                return default(T);
            }
            return (T)obj;
        }
        public void SetValue<T>(string field, T value)
        {
            if (_attrs.ContainsKey(field))
            {
                _attrs[field] = value;
            }
            else
            {
                _attrs.Add(field, value);
            }
        }
        [JsonIgnore]
        public Dictionary<object, object> Attrs
        {
            get { return _attrs; }
        }
        [JsonIgnore]
        public string[] Keys {
            get {
                return _attrs.Keys.Select(m => m.ToString()).ToArray();
            }
        }
        public object this[string key] {
            get {
                object obj = null;
                if (_attrs.TryGetValue(key,out obj))
                {
                    return obj;
                }
                return obj;
            }
            set {
                if (_attrs.Any(c => string.Compare(c.Key.ToString(), key, true) != -1))
                {
                    _attrs[key] = value;
                }
                else
                {
                    _attrs.Add(key, value);
                }
            }
        }
        public Guid Id {
            get {
                return GetValue<Guid>("Id");
            }
            set {
                SetValue<Guid>("Id", value);
            }
        }
        public DateTime CreateTime {
            get {
                return GetValue<DateTime>("CreateTime");
            }
            set {
                SetValue<DateTime>("CreateTime", value);
            }
        }
        [Timestamp]
        [JsonIgnore]
        public byte[] Version { get; set; }

    }
}
