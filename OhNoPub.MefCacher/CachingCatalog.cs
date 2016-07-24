using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhNoPub.MefCacher
{
    /// <summary>
    ///   Wraps a <see cref="ComposablePartCatalog"/> with a cache that intercepts
    ///   part scans, loads them from a cache if found, or updates the cache from
    ///   the underlying catalog if necessary, and loads parts from the underlying
    ///   catalog when they need to be loaded.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     This is a generic caching catalog. It needs to know when the cache
    ///     should be expired, access to a cache, and the underlying catalog.
    ///     It is recommended to use one of the catalogues which tries to automagically wrap
    ///     an official catalog in a meaningful way, such as <see cref="CachingAssemblyCatalog"/>
    ///     or <see cref="CachingDirectoryCatalog"/> or <see cref="CachingApplicationCatalog"/>
    ///     if that fits your needs.
    ///   </para>
    /// </remarks>
    public class CachingCatalog
        : ComposablePartCatalog
    {
        ComposablePartCatalog UnderlyingCatalog { get; }
        IPartCache Cache { get; }
        ICatalogVersionSource CatalogVersionSource { get; }

        /// <summary>
        ///   Build a caching catalog.
        /// </summary>
        /// <param name="catalog">The catalog to wrap. Must behave deterministically, may not dynamically change its offerings while wrapped.</param>
        /// <param name="cache">The cache in which to store and whence toretrieve part information.</param>
        /// <param name="versionSource">Way to detect changes in the catalog.</param>
        public CachingCatalog(
            ComposablePartCatalog catalog,
            IPartCache cache,
            ICatalogVersionSource versionSource)
        {
            if (catalog == null) throw new ArgumentNullException(nameof(catalog));
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            if (versionSource == null) throw new ArgumentNullException(nameof(versionSource));

            UnderlyingCatalog = catalog;
            Cache = cache;
            CatalogVersionSource = versionSource;

            // Do not return stale entries.
            Cache.AssertVersion(
                versionSource.GetVersion(
                    catalog));
        }

        public override IEnumerator<ComposablePartDefinition> GetEnumerator()
        {
            var lazyUnderlyingDefinitions = new Lazy<IEnumerable<ComposablePartDefinition>>(() => UnderlyingCatalog);
            var definitions = Cache.GetDefinitions(lazyUnderlyingDefinitions);
            if (definitions == null)
            {
                Cache.SetDefinitions(
                    UnderlyingCatalog);
                definitions = Cache.GetDefinitions(lazyUnderlyingDefinitions);
            }
            return definitions.GetEnumerator();
        }
    }
}
