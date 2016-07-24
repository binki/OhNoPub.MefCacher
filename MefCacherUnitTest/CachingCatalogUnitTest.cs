using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OhNoPub.MefCacher;
using System.ComponentModel.Composition.Hosting;
using OhNoPub.MefCacherUnitTest.Parts;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.Linq;

namespace OhNoPub.MefCacherUnitTest
{
    [TestClass]
    public class CachingCatalogUnitTest
    {
        [TestMethod]
        public void Functionality_TypeCatalog_MemoryCache() => TypeCatalogAssert(
            new MemoryPartCache(),
            instantiationShouldQueryLower: false);
        [TestMethod]
        public void Functionality_TypeCatalog_SerializingCache() => TypeCatalogAssert(
            new SerializingStoringPartCache(
                new MemoryStreamCacheStorage(),
                new SimplePartSerializer()),
            instantiationShouldQueryLower: true);

        class Things
        {
            public object InitialUnderlyingCatalogToken { get; set; }
            public InterceptingCatalog InterceptingUnderlyingCatalog { get; set; }

            public object InitialCachingCatalogToken { get; set; }
            public InterceptingCatalog InterceptingCachingCatalog { get; set; }
        }

        void UseThings(
            IPartCache cache,
            Action<Things> action)
        {
            using (var underlyingCatalog = new TypeCatalog(typeof(PartA), typeof(PartB)))
            using (var interceptingUnderlyingCatalog = new InterceptingCatalog(underlyingCatalog))
            using (var cachingCatalog = new CachingCatalog(
                interceptingUnderlyingCatalog,
                cache,
                new ConstantCatalogVersionSource("asdf")))
            using (var interceptingCachingCatalog = new InterceptingCatalog(cachingCatalog))
                action(
                    new Things
                    {
                        InitialUnderlyingCatalogToken = interceptingUnderlyingCatalog.EnumerationToken,
                        InterceptingUnderlyingCatalog = interceptingUnderlyingCatalog,
                        InitialCachingCatalogToken = interceptingCachingCatalog.EnumerationToken,
                        InterceptingCachingCatalog = interceptingCachingCatalog,
                    });
        }

        void TypeCatalogAssert(IPartCache cache, bool instantiationShouldQueryLower)
        {
            UseThings(
                cache,
                things =>
                {
                    using (var container = new CompositionContainer(things.InterceptingCachingCatalog))
                    {
                        var b = container.GetExportedValue<PartB>();
                        Assert.IsNotNull(b);
                        Assert.IsNotNull(b.PartA);

                        // Expect both the cacher and the underlying catalog to have been
                        // queried.
                        Assert.AreNotSame(things.InitialUnderlyingCatalogToken, things.InterceptingUnderlyingCatalog.EnumerationToken);
                        Assert.AreNotSame(things.InitialCachingCatalogToken, things.InterceptingCachingCatalog.EnumerationToken);
                    }
                });
            UseThings(
                cache,
                things =>
                {
                    // Try querying the container for a part known not to be in the catalog.
                    // The point of the Cache is to avoid defering to the underlying real
                    // catalog when it is known that the real catalog could not satisfy the
                    // request. If the token stays constant here, we have succeeded.
                    using (var typeCatalog = new TypeCatalog(typeof(ImportsManyNonexistent)))
                    using (var aggregateCatalog = new AggregateCatalog(typeCatalog, things.InterceptingCachingCatalog))
                    using (var container2 = new CompositionContainer(aggregateCatalog))
                    {
                        var importsManyNonexistent = container2.GetExportedValue<ImportsManyNonexistent>();
                        Assert.IsNotNull(importsManyNonexistent);
                        Assert.IsNotNull(importsManyNonexistent.ShouldBeEmpty);
                        Assert.IsFalse(importsManyNonexistent.ShouldBeEmpty.Any());

                        // Expect the cacher to be queried but not the underlying.
                        Assert.AreSame(things.InitialUnderlyingCatalogToken, things.InterceptingUnderlyingCatalog.EnumerationToken);
                        Assert.AreNotSame(things.InitialCachingCatalogToken, things.InterceptingCachingCatalog.EnumerationToken);
                    }
                });

            UseThings(
                cache,
                things =>
                {
                    // Verify that instantiation actually works.
                    using (var container3 = new CompositionContainer(things.InterceptingCachingCatalog))
                    {
                        var b = container3.GetExportedValue<PartB>();
                        Assert.IsNotNull(b);
                        Assert.IsNotNull(b.PartA);

                        // Expect just the cacher to have been queried and permit the underlying
                        // catalog to have been queried. If the cache reports that an item is
                        // in it, it still needs to defer to the underlying catalog for actual part
                        // creation.
                        if (instantiationShouldQueryLower)
                            Assert.AreNotSame(things.InitialUnderlyingCatalogToken, things.InterceptingUnderlyingCatalog.EnumerationToken);
                        else
                            Assert.AreSame(things.InitialUnderlyingCatalogToken, things.InterceptingUnderlyingCatalog.EnumerationToken);

                        Assert.AreNotSame(things.InitialCachingCatalogToken, things.InterceptingCachingCatalog.EnumerationToken);
                    }
                });
        }

        [Export]
        class ImportsManyNonexistent
        {
            [ImportMany]
            public IEnumerable<NonExportedPart> ShouldBeEmpty { get; set; }

            public class NonExportedPart { }
        }
    }
}
