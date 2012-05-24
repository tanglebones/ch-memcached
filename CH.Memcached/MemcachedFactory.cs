using CH.IoC.Infrastructure.Wiring;

namespace CH.Memcached
{
    [Wire]
    internal class MemcachedFactory : IMemcachedFactory
    {
        private readonly IMemcachedClientFactory _memcachedClientFactory;

        public MemcachedFactory(IMemcachedClientFactory memcachedClientFactory)
        {
            _memcachedClientFactory = memcachedClientFactory;
        }

        public IMemcached Create(IMemcachedSettings memcachedSettings)
        {
            return new Memcached(memcachedSettings,_memcachedClientFactory);
        }
    }
}