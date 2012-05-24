namespace CH.Memcached
{
    public interface IMemcachedFactory
    {
        IMemcached Create(IMemcachedSettings memcachedSettings);
    }
}