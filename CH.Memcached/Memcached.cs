using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using CH.IoC.Infrastructure.Wiring;
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace CH.Memcached
{
    /// <summary>
    ///   CH.Memcached implements a TryGetOrAdd pattern on top of Enyim.Caching.Memcached The TryGetOrAdd pattern uses a lock key in the cache to prevent multiple instances from producing the same value Only string values are supported, since this makes it clearer than using object.ToString() internally would. An attempt to read from the cache is given a function to compute the value if it is missing If the value is in the cache it is returned in value and the function returns true If the value is missing the reader attempts to acquire the producer lock If they acquire the producer lock the function to produce the value is executed and the result stored in the cache. Value is set and the function returns true If they do not acquire the producer lock the function returns false and value is null The lock ttl should be longer than the time it takes to compute the value and shorted than the TtlMin. TtlRange is used to smooth out the occurance of misses due to cache expiry and should be ~ 2 time TtlMin, or more. To ensure the hash keys are not too large and are valid for memcached the SHA1 of the prefix and the key is used. The lock key for a key is the hash prefixed with 'L'.
    /// </summary>
    [Wire]
    public class Memcached : IMemcached
    {
        private static readonly Random Random = new Random();
        private readonly IMemcachedSettings _memcacheSettings;
        private readonly IMemcachedClient _memcachedClient;

        public Memcached(IMemcachedSettings memcachedSettings, IMemcachedClientFactory memcachedClientFactory)
        {
            _memcacheSettings = memcachedSettings;
            _memcachedClient = memcachedClientFactory.Create(memcachedSettings);
        }

        public IDictionary<string,IDictionary<string,string>> Stats()
        {
            var serverStats = _memcachedClient.Stats();
            var result = new Dictionary<string, IDictionary<string, string>>();

            // yep, total hack.
            var type = typeof (ServerStats);
            var fieldInfo = type.GetField("results",BindingFlags.Instance|BindingFlags.NonPublic);
            if (fieldInfo == null) return result;

            var results = (Dictionary<IPEndPoint, Dictionary<string, string>>) fieldInfo.GetValue(serverStats);
            if (results == null) return result;

            foreach(var server in results)
            {
                var dictionary = new Dictionary<string, string>();
                result[server.Key.ToString()] = dictionary;
                foreach(var stat in server.Value)
                {
                    dictionary[stat.Key] = stat.Value;
                }
            }

            return result;
        }

        public bool TryGetOrAdd(string key, Func<string> valueProducer, out string value)
        {
            var keyHash = Hash(key);

            if (Get(keyHash, out value))
                return true;

            var lockHash = "L" + keyHash;
            if (GetLock(lockHash))
            {
                value = valueProducer();
                var ret = Put(keyHash, value);
                Delete(lockHash);
                return ret;
            }

            if (!Put("ping", string.Empty))
                throw new Exception("Lost connection?");

            return false;
        }

        private void Delete(string lockHash)
        {
            _memcachedClient.Remove(lockHash);
        }

        private bool Put(string keyHash, string value)
        {
            return _memcachedClient.Store(StoreMode.Set, keyHash, Encoding.UTF8.GetBytes(value), Ttl());
        }

        private TimeSpan Ttl()
        {
            return
                _memcacheSettings.TtlMin.Add(
                    TimeSpan.FromMilliseconds(
                        Random.NextDouble()*_memcacheSettings.TtlRange.Milliseconds
                        )
                    );
        }

        private bool GetLock(string lockHash)
        {
            return _memcachedClient.Store(StoreMode.Add, lockHash, string.Empty, _memcacheSettings.LockTime);
        }

        private bool Get(string keyHash, out string value)
        {
            value = null;
            var data = _memcachedClient.Get<byte[]>(keyHash);
            if (data == null)
                return false;
            value = Encoding.UTF8.GetString(data);
            return true;
        }

        private string Hash(string key)
        {
            using (var sha1 = new SHA1CryptoServiceProvider())
            {
                var buffer = Encoding.UTF8.GetBytes(_memcacheSettings.Prefix + '\0' + key);
                return BitConverter.ToString(sha1.ComputeHash(buffer, 0, buffer.Length)).Replace("-", string.Empty);
            }
        }
    }
}