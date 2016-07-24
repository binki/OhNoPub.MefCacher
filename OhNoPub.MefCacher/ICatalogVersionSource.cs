using System.ComponentModel.Composition.Primitives;

namespace OhNoPub.MefCacher
{
    public interface ICatalogVersionSource
    {
        /// <summary>
        ///   Generate a string that uniquely identifies the set of parts returned by the catalog.
        ///   This string is used to detect if the cache is valid for the catalog in its current
        ///   state (i.e., to detect whether an assembly has been changed).
        /// </summary>
        /// <returns>An opaque string uniquely identifying the catalog.</returns>
        string GetVersion(ComposablePartCatalog catalog);
    }
}
