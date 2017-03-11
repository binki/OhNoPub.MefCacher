using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace OhNoPub.MefCacher.Serialization
{
    public class AggregatingDataContractSurrogate
        : IInfoDataContractSurrogate
    {
        IInfoDataContractSurrogate[] Surrogates { get; }

        public IEnumerable<Type> KnownTypes => Surrogates.SelectMany(s => s.KnownTypes);

        public AggregatingDataContractSurrogate(
            params IInfoDataContractSurrogate[] surrogates)
        {
            Surrogates = surrogates.ToArray();
        }

        public object GetCustomDataToExport(MemberInfo memberInfo, Type dataContractType)
            => (from s in Surrogates let o = s.GetCustomDataToExport(memberInfo, dataContractType) where o != null select o).SingleOrDefault();

        public object GetCustomDataToExport(Type clrType, Type dataContractType)
            => (from s in Surrogates let o = s.GetCustomDataToExport(clrType, dataContractType) where o != null select o).SingleOrDefault();

        public Type GetDataContractType(Type type)
        {
            foreach (var s in Surrogates)
                type = s.GetDataContractType(type);
            return type;
        }

        public object GetDeserializedObject(object obj, Type targetType)
        {
            // In reverse.
            foreach (var s in Surrogates.Reverse())
                obj = s.GetDeserializedObject(obj, targetType);
            return obj;
        }

        public void GetKnownCustomDataTypes(Collection<Type> customDataTypes)
        {
            foreach (var s in Surrogates)
                s.GetKnownCustomDataTypes(customDataTypes);
        }

        public object GetObjectToSerialize(object obj, Type targetType)
        {
            foreach (var s in Surrogates)
                obj = s.GetObjectToSerialize(obj, targetType);
            return obj;
        }

        public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
            => (from s in Surrogates let t = s.GetReferencedTypeOnImport(typeName, typeNamespace, customData) where t != null select t).SingleOrDefault();

        public CodeTypeDeclaration ProcessImportedType(CodeTypeDeclaration typeDeclaration, CodeCompileUnit compileUnit)
        {
            // In reverse?
            foreach (var s in Surrogates.Reverse())
                typeDeclaration = s.ProcessImportedType(typeDeclaration, compileUnit);
            return typeDeclaration;
        }
    }
}
