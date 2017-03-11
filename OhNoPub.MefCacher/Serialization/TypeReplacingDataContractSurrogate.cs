using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace OhNoPub.MefCacher.Serialization
{
    /// <summary>
    ///   A base class for surrogates only interested in simple type replacement.
    /// </summary>
    public class TypeReplacingDataContractSurrogate
        : IInfoDataContractSurrogate
    {
        /// <summary>
        ///   You should concat these to your serializer’s list of known types to work
        ///   with this surrogate.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     The surrogate type will be listed here so that if the type being surrogated
        ///     is not discoverable by static analysis of <see cref="DataContractSerializer"/>’s
        ///     constructor you can stlil serialize/deserialize this type. E.g., if you have
        ///     a non-generic list the serializer cannot guess that you might put this
        ///     thing’s surrogate into it.
        ///   </para>
        /// </remarks>
        public virtual IEnumerable<Type> KnownTypes { get; }

        public object GetCustomDataToExport(MemberInfo memberInfo, Type dataContractType)
            => null;

        public object GetCustomDataToExport(Type clrType, Type dataContractType)
            => null;

        public virtual Type GetDataContractType(Type type)
            => type;

        public virtual object GetDeserializedObject(object obj, Type targetType)
            => obj;

        public void GetKnownCustomDataTypes(Collection<Type> customDataTypes)
        {
        }

        public virtual object GetObjectToSerialize(object obj, Type targetType)
            => obj;

        public virtual Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
            => null;

        public CodeTypeDeclaration ProcessImportedType(CodeTypeDeclaration typeDeclaration, CodeCompileUnit compileUnit)
            => typeDeclaration;
    }
}
