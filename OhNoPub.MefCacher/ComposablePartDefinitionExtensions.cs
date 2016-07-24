using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhNoPub.MefCacher
{
    static class ComposablePartDefinitionExtensions
    {
        /// <summary>
        ///   Generate a string representing the signature of a <see cref="ComposablePartDefinition"/>.
        ///   This is used to perform a hash join between cached definitions and real definitions
        ///   so that they can be matched up when it is determined that the parts wrapped by the
        ///   cache actually need to be loaded.
        /// </summary>
        public static string GetSignature(this ComposablePartDefinition definition)
        {
            return "ed:" + Quote(definition.ExportDefinitions.GetSignature())
                + " id: " + Quote(definition.ImportDefinitions.GetSignature())
                + " m: " + Quote(definition.Metadata.GetSignature());
        }

        static string GetSignature(this IEnumerable<ExportDefinition> exportDefinitions)
        {
            return string.Concat(
                from ed in exportDefinitions
                let signature = ed.GetSignature()
                orderby signature
                select Quote(signature));
        }

        public static string GetSignature(this ExportDefinition exportDefinition)
        {
            return Quote(exportDefinition.ContractName)
                + Quote(exportDefinition.Metadata.GetSignature());
        }

        static string GetSignature(this IEnumerable<ImportDefinition> importDefinitions)
        {
            return string.Concat(
                from id in importDefinitions
                let signature = id.GetSignature()
                orderby signature
                select Quote(signature));
        }

        public static string GetSignature(this ImportDefinition importDefinition)
        {
            return Quote(importDefinition.ContractName)
                + (int)importDefinition.Cardinality
                + (importDefinition.IsPrerequisite ? 1 : 0)
                + (importDefinition.IsRecomposable ? 1 : 0)
                // In fact, we’re unlikely to support constraints at all… depends on the serializer.
                // Or we will have to also have a custom ImportDefinition for serialization which
                // avoids serializing this property and defers to the real object (since when MEF
                // is evaluating the import constriants it is probably looking to actaully instantiate
                // that part anyway). In fact, because it defers to the real object, we can’t include
                // in our signature calculation—to do so, we would need to use a different property
                //+ Quote($"{importDefinition.Constraint}")
                + Quote(importDefinition.Metadata.GetSignature())
                ;
        }

        static string GetSignature(this IDictionary<string, object> metadata)
        {
            // Metadata’ll be the hardest to get right. For now even depending on
            // metadata values having sane .ToString() implementations. Not sure
            // if maybe I should rely on the fact that they need to Equal() and
            // will thus have working GetHashCode(). In fact, I will rely on that
            // to supplement their ToString().
            //
            // Another thing is that dictionaries do not have well defined ordering
            // but to compare dictionaries by string signature I need to define an
            // ordering. Not hard with string keys, use ordinal (just whatever orderby
            // does should work).
            return string.Join( // prettier than concat though pointless
                ",",
                from kvp in metadata
                orderby kvp.Key
                select $"{Quote(kvp.Key)}: {kvp.Value?.GetHashCode()}={kvp.Value}");
        }

        /// <summary>
        ///   Quote the string so that Quote(a)+Quote(b) can only ever equal
        ///   Quote(c)+Quote(d) if a equals c and b equals d.
        /// </summary>
        static string Quote(string s) => $"\"{s.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"";
    }
}
