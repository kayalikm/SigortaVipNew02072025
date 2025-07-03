using System;
using System.Collections.Generic;

namespace SigortaVipNew.Helpers
{
    public class SimpleCache
    {
        private readonly Dictionary<string, CacheItem> _cache;
        private readonly object _lockObject;

        public SimpleCache()
        {
            _cache = new Dictionary<string, CacheItem>();
            _lockObject = new object();
        }

        // Cache'e veri ekle
        public void Set(string key, object value, TimeSpan? expiration = null)
        {
            if (string.IsNullOrEmpty(key) || value == null)
                return;

            lock (_lockObject)
            {
                var expirationTime = expiration.HasValue
                    ? DateTime.Now.Add(expiration.Value)
                    : DateTime.Now.AddHours(1); // Varsayılan 1 saat

                _cache[key] = new CacheItem(value, expirationTime);

                ErrorLogger.LogError($"Cache'e eklendi: {key}, Süre: {expirationTime}");
            }
        }

        // Cache'den veri al
        public T Get<T>(string key) where T : class
        {
            if (string.IsNullOrEmpty(key))
                return null;

            lock (_lockObject)
            {
                if (_cache.TryGetValue(key, out CacheItem item))
                {
                    if (item.ExpirationTime > DateTime.Now)
                    {
                        ErrorLogger.LogError($"Cache'den alındı: {key}");
                        return item.Value as T;
                    }
                    else
                    {
                        // Süresi geçmiş, sil
                        _cache.Remove(key);
                        ErrorLogger.LogError($"Cache süresi geçmiş, silindi: {key}");
                    }
                }
            }

            return null;
        }

        // Cache'de var mı kontrol et
        public bool Exists(string key)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            lock (_lockObject)
            {
                if (_cache.TryGetValue(key, out CacheItem item))
                {
                    if (item.ExpirationTime > DateTime.Now)
                    {
                        return true;
                    }
                    else
                    {
                        _cache.Remove(key);
                    }
                }
            }

            return false;
        }

        // Cache'i temizle
        public void Clear()
        {
            lock (_lockObject)
            {
                int count = _cache.Count;
                _cache.Clear();
                ErrorLogger.LogError($"Cache temizlendi, {count} öğe silindi");
            }
        }

        // Süresi geçmiş öğeleri temizle
        public void CleanExpired()
        {
            lock (_lockObject)
            {
                var keysToRemove = new List<string>();
                var now = DateTime.Now;

                foreach (var kvp in _cache)
                {
                    if (kvp.Value.ExpirationTime <= now)
                    {
                        keysToRemove.Add(kvp.Key);
                    }
                }

                foreach (var key in keysToRemove)
                {
                    _cache.Remove(key);
                }

                if (keysToRemove.Count > 0)
                {
                    ErrorLogger.LogError($"Süresi geçen {keysToRemove.Count} cache öğesi temizlendi");
                }
            }
        }

        private class CacheItem
        {
            public object Value { get; }
            public DateTime ExpirationTime { get; }

            public CacheItem(object value, DateTime expirationTime)
            {
                Value = value;
                ExpirationTime = expirationTime;
            }
        }
    }
}