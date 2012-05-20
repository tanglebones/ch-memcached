using System;
using System.Collections.Generic;

namespace CH.Memcached
{
    namespace Ex
    {
        public static class MemcachedSettings
        {
            public static IMemcachedSettings Settings
            {
                get { return new MemcachedSettingsImpl(); }
            }

            public static IMemcachedSettings Prefix(this IMemcachedSettings settings, string value)
            {
                settings.Prefix = value;
                return settings;
            }

            public static IMemcachedSettings LockTime(this IMemcachedSettings settings, TimeSpan value)
            {
                settings.LockTime = value;
                return settings;
            }

            public static IMemcachedSettings TtlMin(this IMemcachedSettings settings, TimeSpan value)
            {
                settings.TtlMin = value;
                return settings;
            }

            public static IMemcachedSettings TtlRange(this IMemcachedSettings settings, TimeSpan value)
            {
                settings.TtlRange = value;
                return settings;
            }

            public static IMemcachedSettings Server(this IMemcachedSettings settings, string host, ushort port)
            {
                settings.Server.Add(Tuple.Create(host, port));
                return settings;
            }

            private sealed class MemcachedSettingsImpl : IMemcachedSettings
            {
                public MemcachedSettingsImpl()
                {
                    Prefix = string.Empty;
                    LockTime = TimeSpan.FromSeconds(60);
                    TtlMin = TimeSpan.FromSeconds(300);
                    TtlRange = TimeSpan.FromSeconds(300);
                    Server = new List<Tuple<string, ushort>>();
                }

                public string Prefix { get; set; }
                public TimeSpan LockTime { get; set; }
                public TimeSpan TtlMin { get; set; }
                public TimeSpan TtlRange { get; set; }
                public IList<Tuple<string, ushort>> Server { get; set; }
            }
        }
    }
}