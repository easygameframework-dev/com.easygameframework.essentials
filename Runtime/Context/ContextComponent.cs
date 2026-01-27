
using System;
using System.Collections.Generic;

namespace EasyGameFramework.Essentials
{
    public class ContextComponent : GameFrameworkComponent
    {
        private readonly Dictionary<string, object> _context = new Dictionary<string, object>();

        public object this[string key]
        {
            get => _context[key];
            set => _context[key] = value;
        }

        public object Get(string key)
        {
            if (!_context.TryGetValue(key, out var value))
            {
                throw new KeyNotFoundException($"Context key '{key}' not found.");
            }

            return value;
        }

        public T Get<T>(string key)
        {
            var value = Get(key);
            if (value is not T result)
            {
                throw new InvalidCastException($"Context key '{key}' is not of type '{typeof(T)}'.");
            }

            return result;
        }

        public bool Has(string key)
        {
            return _context.ContainsKey(key);
        }

        public void Set(string key, object value)
        {
            _context[key] = value;
        }

        public void Remove(string key)
        {
            _context.Remove(key);
        }

        public void Clear()
        {
            _context.Clear();
        }
    }
}
