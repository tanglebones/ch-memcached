using System;
using CH.Memcached.Ex;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using FakeItEasy;
using NUnit.Framework;

namespace CH.Memcached.Test
{
    [TestFixture]
    public class TestFixture
    {
        [Test]
        public void Test()
        {
            var mcf = A.Fake<IMemcachedClientFactory>();
            var mcc = A.Fake<IMemcachedClient>();
            A.CallTo(() => mcf.Create(A<IMemcachedSettings>._)).Returns(mcc);
            A.CallTo(() => mcc.Get<byte[]>(A<string>._)).Returns(null);
            A.CallTo(() => mcc.Store(StoreMode.Add, A<string>._, A<object>._, A<TimeSpan>._)).Returns(true);

            var chmc = new Memcached(
                MemcachedSettings.Settings
                    .Prefix("test")
                    .Server("localhost", 12345)
                    .TtlMin(TimeSpan.FromSeconds(30))
                    .TtlRange(TimeSpan.FromSeconds(3000))
                    .LockTime(TimeSpan.FromSeconds(15))
                , mcf
                );

            string value;
            var result = chmc.TryGetOrAdd("test", () => "value", out value);

            A.CallTo(() => mcc.Store(StoreMode.Set, A<string>._, A<object>._, A<TimeSpan>._)).MustHaveHappened();
            A.CallTo(() => mcc.Remove(A<string>._)).MustHaveHappened();
        }
    }
}