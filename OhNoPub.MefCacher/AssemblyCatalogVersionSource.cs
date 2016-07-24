using System;
using System.ComponentModel.Composition.Primitives;
using System.IO;

namespace OhNoPub.MefCacher
{
    public class AssemblyCatalogVersionSource
        : ICatalogVersionSource
    {
        string Filename { get; }

        public string GetVersion(ComposablePartCatalog catalog)
        {
            var info = new FileInfo(Filename);
            return $"{info.Length} {info.LastWriteTimeUtc:o}";
        }

        public AssemblyCatalogVersionSource(
            string filename)
        {
            Filename = filename;
        }
    }
}
