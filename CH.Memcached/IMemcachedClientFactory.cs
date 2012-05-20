using Enyim.Caching;

namespace CH.Memcached
{
    public interface IMemcachedClientFactory
    {
        IMemcachedClient Create(IMemcachedSettings memcachedSettings);
    }
}