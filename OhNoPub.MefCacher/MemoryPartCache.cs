using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhNoPub.MefCacher
{
    public class MemoryPartCache : IPartCache
    {
        readonly object propertyLock = new object();
        string Version { get; set; }
        IEnumerable<ComposablePartDefinition> Parts { get; set; }

        public void AssertVersion(string version)
        {
            lock (propertyLock)
            {
                if (Version != version)
                {
                    Parts = null;
                    Version = version;
                }
            }
        }

        public IEnumerable<ComposablePartDefinition> GetDefinitions(
            Lazy<IEnumerable<ComposablePartDefinition>> lazyUnderlyingDefinitions)
        {
            lock (propertyLock)
                return Parts;
        }

        public void SetDefinitions(IEnumerable<ComposablePartDefinition> enumerator)
        {
            lock (propertyLock)
                Parts = enumerator.ToList().AsReadOnly();
        }
    }
}
