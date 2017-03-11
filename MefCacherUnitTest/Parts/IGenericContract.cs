using System.ComponentModel.Composition;

namespace OhNoPub.MefCacherUnitTest.Parts
{
    [InheritedExport]
    interface IGenericContract<T, U>
    {
        T Value { get; }
        U OtherValue { get; }
    }
}
