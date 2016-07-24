using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhNoPub.MefCacher
{
    public interface IPartSerializer
    {
        /// <summary>
        ///   Stream to serialize to or deserialize from.
        /// </summary>
        Stream Stream { set; }
        
        /// <summary>
        ///   Serialize the definitions to <see cref="Stream"/> 
        /// </summary>
        /// <param name="definitions"></param>
        void Serialize(IEnumerable<ComposablePartDefinition> definitions);

        /// <summary>
        ///   Deserialize the definitions from <see cref="Stream"/>. The
        ///   caller guarantees that it will finish enumerating (in case
        ///   the implementor needs to clean stuff up).
        /// </summary>
        /// <param name="lazyUnderlyingCatalog">
        ///   The <see cref="ComposablePartDefinition"/>s returned by this method
        ///   must be able to lazily load the underlying definitions at some point
        ///   so that the cache-provided definitions can actually instantiate the
        ///   real parts when the environment decides it actually needs them. This
        ///   paramter provides this binding.
        /// </param>
        IEnumerable<ComposablePartDefinition> Deserialize(
            Lazy<IEnumerable<ComposablePartDefinition>> lazyUnderlyingCatalog);
    }
}
