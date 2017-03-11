using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace OhNoPub.MefCacher.Serialization
{
    /// <summary>
    ///   Simple surrogate for <see cref="Type"/> which uses <see cref="Type.GetType(string)"/>.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     This simple implementation will work for most “real” types without
    ///     any issue but at the expense of triggering assembly loads for types
    ///     defined in unloaded assemblies.
    ///   </para>
    /// </remarks>
    public class TypeDataContractSurrogate
        : TypeReplacingDataContractSurrogate
    {
        public override IEnumerable<Type> KnownTypes => new[] {
            typeof(Type),
            typeof(SType),
            typeof(STypeArray)
        };

        public override Type GetDataContractType(Type type)
        {
            if (typeof(Type).IsAssignableFrom(type))
                return typeof(SType);
            if (typeof(Type[]).IsAssignableFrom(type))
                return typeof(STypeArray);
            return base.GetDataContractType(type);
        }

        public override object GetDeserializedObject(object obj, Type targetType)
            => (obj as SType)?.ToType()
            ?? (obj as STypeArray)?.ToTypes()
            ?? obj;

        public override object GetObjectToSerialize(object obj, Type targetType)
        {
            var type = obj as Type;
            if (type != null)
                return SType.FromType(type);
            var typeArray = obj as Type[];
            if (typeArray != null)
                return new STypeArray(typeArray.ToList());
            return base.GetObjectToSerialize(obj, targetType);
        }

        public override Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
        {
            const string myNamespace = "OhNoPub.MefCacher.Serialization";
            if (typeName == nameof(SType) && typeNamespace == myNamespace)
                return typeof(Type);
            if (typeName == nameof(STypeArray) && typeNamespace == myNamespace)
                return typeof(Type[]);
            return base.GetReferencedTypeOnImport(typeName, typeNamespace, customData);
        }

        [DataContract]
        class STypeArray
        {
            [DataMember]
            List<Type> Things { get; set; }

            public STypeArray(
                List<Type> things)
            {
                Things = things;
            }

            public object ToTypes()
                => Things.ToArray();
        }

        [DataContract]
        class SType
        {
            [DataMember]
            string AssemblyQualifiedName { get; set; }

            SType(
                string assemblyQualitifedName)
            {
                AssemblyQualifiedName = assemblyQualitifedName;
            }

            public Type ToType() => Type.GetType(AssemblyQualifiedName);

            public static SType FromType(Type type)
                => type == null ? null : new SType(
                    type.AssemblyQualifiedName);
        }
    }
}
