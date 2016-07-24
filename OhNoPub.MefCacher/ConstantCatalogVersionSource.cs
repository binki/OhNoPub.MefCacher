using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhNoPub.MefCacher
{
    public class ConstantCatalogVersionSource : ICatalogVersionSource
    {
        public string Version { get; }

        public ConstantCatalogVersionSource(
            string version)
        {
            Version = version;
        }

        public string GetVersion(ComposablePartCatalog catalog) => Version;
    }
}
