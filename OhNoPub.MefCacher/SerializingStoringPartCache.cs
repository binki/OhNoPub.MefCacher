using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhNoPub.MefCacher
{
    /// <summary>
    ///   This cache is pretty stupid. It needs a 
    /// </summary>
    public class SerializingStoringPartCache
        : IPartCache
    {
        string AssertedVersion { get; set; }

        ICacheStorage Storage { get; }
        IPartSerializer Serializer { get; }
        public SerializingStoringPartCache(
            ICacheStorage storage,
            IPartSerializer serializer)
        {
            Storage = storage;
            Serializer = serializer;
        }

        public void AssertVersion(string version)
        {
            AssertedVersion = version;
        }

        public IEnumerable<ComposablePartDefinition> GetDefinitions(
            Lazy<IEnumerable<ComposablePartDefinition>> lazyUnderlyingDefinitions)
        {
            if (AssertedVersion == null)
                throw new InvalidOperationException("No cache version asserted.");

            string version;
            var stream = Storage.GetReadStream(AssertedVersion, out version);
            try
            {
                if (AssertedVersion != version)
                    return null;

                var passedStream = stream;
                stream = null;
                return GetDefinitionsGenerator(
                    passedStream,
                    lazyUnderlyingDefinitions);
            }
            finally
            {
                stream?.Dispose();
            }
        }

        // Use generator syntax to let us use using (){} even though we barely use it…
        IEnumerable<ComposablePartDefinition> GetDefinitionsGenerator(
            Stream passedStream,
            Lazy<IEnumerable<ComposablePartDefinition>> lazyUnderlyingDefinitions)
        {
            using (var stream = passedStream)
            {
                try
                {
                    Serializer.Stream = stream;
                    foreach (var part in Serializer.Deserialize(lazyUnderlyingDefinitions))
                        yield return part;
                }
                finally
                {
                    Serializer.Stream = null;
                }
            }
        }

        public void SetDefinitions(
            IEnumerable<ComposablePartDefinition> definitions)
        {
            if (AssertedVersion == null)
                throw new InvalidOperationException("No cache version asserted.");

            using (var stream = Storage.GetWriteStream(AssertedVersion))
            {
                try
                {
                    Serializer.Stream = stream;
                    Serializer.Serialize(definitions);
                }
                finally
                {
                    Serializer.Stream = null;
                }
            }
        }
    }
}
