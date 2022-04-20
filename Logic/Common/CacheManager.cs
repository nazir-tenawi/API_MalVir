using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Web;
using System.Runtime.Caching;

namespace MalVirDetector_CLI_API.Logic
{
    public static class CacheManager
    {
        private static MemoryCache _cache = MemoryCache.Default;

        public static bool Contains(string key)
        {
            return _cache.Contains(key);
        }

        public static void AddToLocal<T>(string UserID, string key, T data, DateTime? expirationTime = null)
        {
            string localKey = UserID == null ? "" : UserID.ToString(); //HttpContext.Current.User.Identity.GetUserId();
            LocalStorage _storage = null;
            if (_cache.Contains(localKey))
            {
                _storage = (LocalStorage)_cache[localKey];
            }
            if (_storage == null)
            {
                _storage = new LocalStorage();
                var policy = new CacheItemPolicy();
                if (expirationTime == null)
                {
                    policy.AbsoluteExpiration = DateTime.Now.AddDays(1);
                }
                else { policy.AbsoluteExpiration = expirationTime.Value; }
                _cache.Set(key, _storage, policy);
            }
            _storage[localKey] = data;
        }

        public static T GetFromLocal<T>(string UserID, string key)
        {
            string localKey = UserID == null ? "" : UserID.ToString();//HttpContext.Current.User.Identity.GetUserId();
            LocalStorage _storage = null;
            if (_cache.Contains(key))
            {
                _storage = (LocalStorage)_cache[key];
            }
            if (_storage == null || !_storage.ContainsKey(localKey))
            {
                return default(T);
            }
            else
            {
                return (T)_storage[localKey];
            }
        }

        public static void ClearAllCache()
        {
            _cache = MemoryCache.Default;
            _cache.Trim(100);
        }

        public static void AddToGlobal<T>(string key, T data, DateTime? expirationTime = null)
        {
            if (_cache.Contains(key))
            {
                _cache[key] = data;
            }
            else
            {
                var policy = new CacheItemPolicy();
                if (expirationTime == null)
                {
                    policy.AbsoluteExpiration = DateTime.Now.AddDays(10);
                }
                else { policy.AbsoluteExpiration = expirationTime.Value; }
                _cache.Set(key, data, policy);
            }
        }

        public static T GetFromGlobal<T>(string key)
        {
            if (_cache.Contains(key))
            {
                return (T)_cache[key];
            }
            else
            {
                return default(T);
            }
        }
    }

    class LocalStorage
    {
        private ConcurrentDictionary<string, object> _dict = new ConcurrentDictionary<string, object>();

        public object this[string key]
        {
            get { return _dict[key]; }
            set { _dict.AddOrUpdate(key, value, (k, old) => { old = value; return value; }); }
        }

        public bool ContainsKey(string key) => _dict.ContainsKey(key);
    }

    public class CacheKeys
    {
        public const string resources = "resources";
        public const string lang = "lang";
    }

}
