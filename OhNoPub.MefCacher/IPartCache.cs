using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;

namespace OhNoPub.MefCacher
{
    public interface IPartCache
    {
        /// <summary>
        ///   Indicate to the cache the version of the catalog
        ///   which will be queried. The cache should only store
        ///   and return values associated with this version.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     It is likely that most caches would record the version
        ///     they were built with and, when detecting that the version
        ///     is changing, flush themselves.
        ///   </para>
        /// </remarks>
        void AssertVersion(
            string version);

        /// <summary>
        ///   Get the cached definitions or null if unavailable.
        /// </summary>
        /// <param name="lazyUnderlyingDefinitions">
        ///   If the code accessing the cache ever tries to realize a part
        ///   instance, the cache-generated ComposablePartDefinition needs
        ///   to be able to trigger loading the real catalog and definitions
        ///   as the cache won’t be able to provide instances.
        /// </param>
        IEnumerable<ComposablePartDefinition> GetDefinitions(
            Lazy<IEnumerable<ComposablePartDefinition>> lazyUnderlyingDefinitions);

        void SetDefinitions(
            IEnumerable<ComposablePartDefinition> enumerable);
    }
}
