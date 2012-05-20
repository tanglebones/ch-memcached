using CH.IoC.Infrastructure.Wiring;
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;

namespace CH.Memcached
{
    [Wire]
    internal class MemcachedClientFactory : IMemcachedClientFactory
    {
        public IMemcachedClient Create(IMemcachedSettings memcachedSettings)
        {
            var mcConfig = new MemcachedClientConfiguration();

            foreach (var server in memcachedSettings.Server)
                mcConfig.AddServer(server.Item1, server.Item2);

            mcConfig.Protocol = MemcachedProtocol.Text;
            return new MemcachedClient(mcConfig);
        }
    }
}