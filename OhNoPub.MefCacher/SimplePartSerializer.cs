using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace OhNoPub.MefCacher
{
    public class SimplePartSerializer
        : IPartSerializer
    {
        DataContractSerializer Serializer { get; } = new DataContractSerializer(typeof(SUniverse));
        public Stream Stream { get; set; }

        public IEnumerable<ComposablePartDefinition> Deserialize(
            Lazy<IEnumerable<ComposablePartDefinition>> lazyUnderlyingCatalog)
        {
            return new UnserializedUniverse(
                lazyUnderlyingCatalog,
                (SUniverse)Serializer.ReadObject(
                    Stream));
        }

        public void Serialize(IEnumerable<ComposablePartDefinition> definitions)
        {
            Serializer.WriteObject(
                Stream,
                new SUniverse(definitions));
        }

        /// <summary>
        ///   Makes a read only Ordinals-keyed metadata dictionary.
        /// </summary>
        static IDictionary<string, object> BakeMetadata(
            IDictionary<string, object> dict)
            => new ReadOnlyDictionary<string, object>(dict);

        // Supports serialization.
        [DataContract]
        internal class SUniverse
        {
            [DataMember]
            public SPart[] Definitions;

            public SUniverse()
            {
            }

            public SUniverse(
                IEnumerable<ComposablePartDefinition> definitions)
            {
                Definitions = definitions.Select(d => new SPart(d)).ToArray();
            }
        }

        class UnserializedUniverse : IEnumerable<ComposablePartDefinition>
        {
            IEnumerable<ComposablePartDefinition> Definitions { get; }
            /// <summary>
            ///   Parts will be stored by <see cref="ComposablePartDefinitionExtensions.GetSignature(ComposablePartDefinition)"/>.
            ///   When an SPart needs to bind itself to its real definition, it should take a definition from
            ///   the bag. If the bag is empty, it should remove it from the dictionary. If the dictionary is
            ///   empty, it should null the lazy.
            /// </summary>
            internal SignatureLookup<ComposablePartDefinition> RealPartLookup;

            public UnserializedUniverse(
                Lazy<IEnumerable<ComposablePartDefinition>> lazyUnderlyingDefinitions,
                SUniverse universe)
            {
                RealPartLookup = new SignatureLookup<ComposablePartDefinition>(
                    lazyUnderlyingDefinitions,
                    ComposablePartDefinitionExtensions.GetSignature);
                Definitions = (
                    from d in universe.Definitions
                    select new UnserializedComposablePartDefinition(
                        this,
                        d)).ToList();
            }

            public IEnumerator<ComposablePartDefinition> GetEnumerator() => Definitions.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [DataContract]
        internal struct SPart
        {
            [DataMember]
            public SExport[] E;
            [DataMember]
            public SImport[] I;
            [DataMember]
            public IDictionary<string, object> M;

            public SPart(ComposablePartDefinition definition)
            {
                E = (from e in definition.ExportDefinitions select new SExport(e)).ToArray();
                I = (from i in definition.ImportDefinitions select new SImport(i)).ToArray();
                M = definition.Metadata;
            }
        }

        class UnserializedComposablePartDefinition : ComposablePartDefinition
        {
            public override IEnumerable<ExportDefinition> ExportDefinitions => Exports.Values;
            // When SetImports() is passed in an ImportDefinition, we need to be able to upcast that to
            // UnserializedExportDefinition to map back to the real one before passing to the real one’s
            // SetImport().
            internal readonly IReadOnlyDictionary<object, UnserializedExportDefinition> Exports;

            public override IEnumerable<ImportDefinition> ImportDefinitions => Imports.Values;
            internal readonly IReadOnlyDictionary<object, UnserializedImportDefinition> Imports;

            public override IDictionary<string, object> Metadata => Meta;
            readonly IDictionary<string, object> Meta;

            // This field links this definition to the real catalog so that
            // the real part can be instantiated. Dereferencing the Lazy causes
            // the real catalog to be loaded. This is not serialized and is
            // bound manually after deserialization.
            readonly internal Lazy<ComposablePartDefinition> LazyRealComposablePartDefinition;
            internal SignatureLookup<ExportDefinition> ExportDefinitionLookup;
            internal SignatureLookup<ImportDefinition> ImportDefinitionLookup;

            public override ComposablePart CreatePart()
                => new CacherComposablePart(this);

            public UnserializedComposablePartDefinition(
                UnserializedUniverse universe,
                SPart sPart)
            {
                Exports = (
                    from e in sPart.E
                    select new UnserializedExportDefinition(this, e))
                    .ToDictionary(e => (object)e);
                Imports = (
                    from i in sPart.I
                    select new UnserializedImportDefinition(this, i))
                    .ToDictionary(i => (object)i);
                Meta = BakeMetadata(sPart.M); // Make deserialized dictionary into ReadOnly

                LazyRealComposablePartDefinition = new Lazy<ComposablePartDefinition>(
                    () => SignatureLookup.Get(ref universe.RealPartLookup, this.GetSignature()));
                ExportDefinitionLookup = new SignatureLookup<ExportDefinition>(
                    new Lazy<IEnumerable<ExportDefinition>>(() => LazyRealComposablePartDefinition.Value.ExportDefinitions),
                    ComposablePartDefinitionExtensions.GetSignature);
                ImportDefinitionLookup = new SignatureLookup<ImportDefinition>(
                    new Lazy<IEnumerable<ImportDefinition>>(() => LazyRealComposablePartDefinition.Value.ImportDefinitions),
                    ComposablePartDefinitionExtensions.GetSignature);
            }
        }
        
        [DataContract]
        internal struct SExport
        {
            [DataMember]
            public string CN;
            [DataMember]
            public IDictionary<string, object> M;

            public SExport(ExportDefinition export)
            {
                CN = export.ContractName;
                M = export.Metadata;
            }
        }
        class UnserializedExportDefinition : ExportDefinition
        {
            public override string ContractName => CN;
            readonly string CN;

            public override IDictionary<string, object> Metadata => M;
            readonly IDictionary<string, object> M;

            internal readonly Lazy<ExportDefinition> LazyRealExportDefinition;

            public UnserializedExportDefinition(
                UnserializedComposablePartDefinition part,
                SExport sExport)
            {
                CN = sExport.CN;
                M = BakeMetadata(sExport.M); // Make deserialized dictionary into ReadOnly

                LazyRealExportDefinition = new Lazy<ExportDefinition>(
                    () => SignatureLookup.Get(ref part.ExportDefinitionLookup, this.GetSignature()));
            }
        }

        [DataContract]
        internal struct SImport
        {
            [DataMember]
            public int Card;
            [DataMember]
            public string CN;
            [DataMember]
            public int Prereq;
            [DataMember]
            public int Recom;
            [DataMember]
            public IDictionary<string, object> M;

            public SImport(ImportDefinition import)
            {
                Card = Convert.ToInt32(import.Cardinality);
                CN = import.ContractName;
                Prereq = Convert.ToInt32(import.IsPrerequisite);
                Recom = Convert.ToInt32(import.IsRecomposable);
                M = import.Metadata;
            }
        }
        [DataContract]
        class UnserializedImportDefinition : ImportDefinition
        {
            public override ImportCardinality Cardinality => Card;
            readonly ImportCardinality Card;
            public override Expression<Func<ExportDefinition, bool>> Constraint => LazyUnderlyingImportDefinition.Value.Constraint;
            public override string ContractName => CN;
            readonly string CN;
            public override bool IsPrerequisite => Prereq;
            readonly bool Prereq;
            public override bool IsRecomposable => Recom;
            readonly bool Recom;
            public override IDictionary<string, object> Metadata => M;
            readonly IDictionary<string, object> M;

            readonly internal Lazy<ImportDefinition> LazyUnderlyingImportDefinition;

            public UnserializedImportDefinition(
                UnserializedComposablePartDefinition part,
                SImport sImport)
            {
                Card = (ImportCardinality)sImport.Card;
                CN = sImport.CN;
                Prereq = Convert.ToBoolean(sImport.Prereq);
                Recom = Convert.ToBoolean(sImport.Recom);
                M = BakeMetadata(sImport.M);

                LazyUnderlyingImportDefinition = new Lazy<ImportDefinition>(
                    () => SignatureLookup.Get(ref part.ImportDefinitionLookup, this.GetSignature()));
            }

            public override bool IsConstraintSatisfiedBy(ExportDefinition exportDefinition)
                => LazyUnderlyingImportDefinition.Value.IsConstraintSatisfiedBy(exportDefinition);
        }

        class CacherComposablePart
            : ComposablePart
        {
            UnserializedComposablePartDefinition Definition { get; }

	    // The composer instantiates ComposablePart before it
	    // needs to instantiate the object. It seems that
	    // CacherComposablePart becomes alive around when the
	    // Lazy<T, TMetadata> appears. So we need to defer loading
	    // yet a little longer to support metadata-based prat
	    // avoidance.
            Lazy<ComposablePart> LazyRealPart { get; }
            ComposablePart RealPart => LazyRealPart.Value;

            public override IEnumerable<ExportDefinition> ExportDefinitions => Definition.ExportDefinitions;

            public override IEnumerable<ImportDefinition> ImportDefinitions => Definition.ImportDefinitions;
            public override IDictionary<string, object> Metadata => Definition.Metadata;

            public CacherComposablePart(
                UnserializedComposablePartDefinition definition)
            {
                Definition = definition;
                LazyRealPart = new Lazy<ComposablePart>(
                    () => Definition.LazyRealComposablePartDefinition.Value.CreatePart());
            }

            public override void Activate()
                => RealPart.Activate();

            public override object GetExportedValue(
                ExportDefinition definition)
            {
                // Need to map this export definition onto the real definition’s export definition.
                UnserializedExportDefinition unserializedExportDefinition;
                if (!Definition.Exports.TryGetValue(definition, out unserializedExportDefinition))
                {
                    // Try to throw similar exception as official MEF implementation would in this scenario.
                    throw new ArgumentOutOfRangeException(nameof(definition), definition, $"definition did not originate from the ExportDefinitions property on this MefCacher’s ComposablePart or its ComposablePartDefinition.");
                }
                var realDefinition = unserializedExportDefinition.LazyRealExportDefinition.Value;
                return RealPart.GetExportedValue(realDefinition);
            }

            public override void SetImport(
                ImportDefinition definition,
                IEnumerable<Export> exports)
            {
                // Need to map this import definition onto the real definition’s import definition.
                // Thankfully we already have done that, but need to scan for this definition…
                UnserializedImportDefinition unserializedImportDefinition;
                if (!Definition.Imports.TryGetValue(definition, out unserializedImportDefinition))
                {
                    // Try to throw the sameish exception that MEF itself throws when giving unknown ImportDefintions.
                    throw new ArgumentOutOfRangeException(nameof(definition), definition, $"definition did not originate from the ImportDefinitions property on this MefCacher’s ComposablePart or its ComposablePartDefinition.");
                }
                var realDefinition = unserializedImportDefinition.LazyUnderlyingImportDefinition.Value;
                RealPart.SetImport(realDefinition, exports);
            }
        }

        internal class SignatureLookup<T>
        {
            public bool IsEmpty => Lookup.Value.IsEmpty;
            Lazy<ConcurrentDictionary<string, ConcurrentBag<T>>> Lookup { get; }
            public SignatureLookup(
                Lazy<IEnumerable<T>> lazyUnderlyingSignables,
                Func<T, string> sign)
            {
                Lookup = new Lazy<ConcurrentDictionary<string, ConcurrentBag<T>>>(
                    // have to iterate through underlying catalog and find matching signature based on imports/exports
                    // and then use that one’s ComposablePartDefinition. Remember that a catalog may contain
                    // multiple parts with the same signature. It doesn’t matter which one we get as long as
                    // there is a one to one mapping between each SPart and each catalog provided part/as long
                    // as we consume all and each only once.
                    () => new ConcurrentDictionary<string, ConcurrentBag<T>>(
                        from signable in lazyUnderlyingSignables.Value
                        group signable by sign(signable) into g
                        select new KeyValuePair<string, ConcurrentBag<T>>(g.Key, new ConcurrentBag<T>(g))));
            }
            public T Get(string signature)
            {
                var bag = Lookup.Value[signature];
                try
                {
                    T value;
                    if (!bag.TryTake(out value))
                        throw new KeyNotFoundException("Bucket for this signature is empty.");
                    return value;
                }
                finally
                {
                    if (bag.IsEmpty)
                        Lookup.Value.TryRemove(signature, out bag);
                }
            }
        }
        static class SignatureLookup
        {
            public static T Get<T>(
                ref SignatureLookup<T> lookup,
                string signature)
            {
                try
                {
                    if (lookup == null)
                        throw new KeyNotFoundException($"Lookup has already been cleared.");
                    return lookup.Get(signature);
                }
                catch (KeyNotFoundException ex)
                {
                    throw new Exception($"Unable to find {typeof(T)} with signature {signature}", ex);
                }
                finally
                {
                    if (lookup.IsEmpty)
                        lookup = null;
                }
            }
        }
    }
}
