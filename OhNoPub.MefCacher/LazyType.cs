using System;
using System.Globalization;
using System.Reflection;

namespace OhNoPub.MefCacher
{
    /// <summary>
    ///   Like <see cref="TypeDelegator"/> but for lazily loaded types
    ///   where getting the actual type information is expensive.
    /// </summary>
    class LazyType : Type
    {
        Lazy<Type> MyLazyType { get; }
        Type Type => MyLazyType.Value;

        public override Assembly Assembly => Type.Assembly;

        public override string AssemblyQualifiedName => Type.AssemblyQualifiedName;

        public override Type BaseType => Type.BaseType;

        public override string FullName => Type.FullName;

        public override Guid GUID => Type.GUID;

        public override Module Module => Type.Module;

        public override string Name => Type.Name;

        public override string Namespace => Type.Namespace;

        public override Type UnderlyingSystemType => Type.UnderlyingSystemType;

        public LazyType(
            Lazy<Type> lazyType)
        {
            MyLazyType = lazyType;
        }

        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
            => Type.GetConstructors(bindingAttr);

        public override object[] GetCustomAttributes(bool inherit)
            => Type.GetCustomAttributes(inherit);

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
            => Type.GetCustomAttributes(attributeType, inherit);

        public override Type GetElementType()
            => Type.GetElementType();

        public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
            => Type.GetEvent(name, bindingAttr);

        public override EventInfo[] GetEvents(BindingFlags bindingAttr)
            => Type.GetEvents(bindingAttr);

        public override FieldInfo GetField(string name, BindingFlags bindingAttr)
            => Type.GetField(name, bindingAttr);

        public override FieldInfo[] GetFields(BindingFlags bindingAttr)
            => Type.GetFields(bindingAttr);

        public override Type GetInterface(string name, bool ignoreCase)
            => Type.GetInterface(name, ignoreCase);

        public override Type[] GetInterfaces()
            => Type.GetInterfaces();

        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
            => Type.GetMembers(bindingAttr);

        public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
            => Type.GetMethods(bindingAttr);

        public override Type GetNestedType(string name, BindingFlags bindingAttr)
            => Type.GetNestedType(name, bindingAttr);

        public override Type[] GetNestedTypes(BindingFlags bindingAttr)
            => Type.GetNestedTypes(bindingAttr);

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
            => Type.GetProperties(bindingAttr);

        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
            => Type.InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);

        public override bool IsDefined(Type attributeType, bool inherit)
            => Type.IsDefined(attributeType, inherit);

        protected override TypeAttributes GetAttributeFlagsImpl()
            => Type.Attributes;

        protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
            => Type.GetConstructor(bindingAttr, binder, callConvention, types, modifiers);

        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
            => Type.GetMethod(name, bindingAttr, binder, callConvention, types, modifiers);

        protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
            => Type.GetProperty(name, bindingAttr, binder, returnType, types, modifiers);

        protected override bool HasElementTypeImpl()
            => Type.HasElementType;

        protected override bool IsArrayImpl()
            => Type.IsArray;

        protected override bool IsByRefImpl()
            => Type.IsByRef;

        protected override bool IsCOMObjectImpl()
            => Type.IsCOMObject;

        protected override bool IsPointerImpl()
            => Type.IsPointer;

        protected override bool IsPrimitiveImpl()
            => Type.IsPrimitive;
    }
}
