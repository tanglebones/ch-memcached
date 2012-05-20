using System;
using System.Collections.Generic;

namespace CH.Memcached
{
    public interface IMemcachedSettings
    {
        string Prefix { get; set; }
        TimeSpan LockTime { get; set; }
        TimeSpan TtlMin { get; set; }
        TimeSpan TtlRange { get; set; }
        IList<Tuple<string, ushort>> Server { get; set; }
    }
}