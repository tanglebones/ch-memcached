using System;
using System.Collections.Generic;

namespace CH.Memcached
{
    public interface IMemcached
    {
        bool TryGetOrAdd(string key, Func<string> valueProducer, out string value);
        bool TryGetOrAdd(string key, Func<string> valueProducer, out string value, bool forceCompute);
        IDictionary<string, IDictionary<string, string>> Stats();
    }
}