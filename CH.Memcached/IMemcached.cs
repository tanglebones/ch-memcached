using System;

namespace CH.Memcached
{
    public interface IMemcached
    {
        bool TryGetOrAdd(string key, Func<string> valueProducer, out string value);
    }
}