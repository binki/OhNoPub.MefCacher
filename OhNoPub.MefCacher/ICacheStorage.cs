using System.IO;

namespace OhNoPub.MefCacher
{
    public interface ICacheStorage
    {
        /// <summary>
        ///   Get a stream representing read access to the data and
        ///   indicate the version of the data stored in the cache.
        /// </summary>
        /// <param name="expectedVersion">Indicates the version the cache is seeking. Useful for keyed caches managing expiration on their own (e.g., one storage for multiple assemblies).</param>
        /// <param name="version"/>
        Stream GetReadStream(string expectedVersion, out string version);

        /// <summary>
        ///   Set the version and get a write stream for that version.
        /// </summary>
        Stream GetWriteStream(string version);
    }
}
