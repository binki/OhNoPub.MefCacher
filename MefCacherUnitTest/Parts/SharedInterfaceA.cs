using System.ComponentModel.Composition;

namespace OhNoPub.MefCacherUnitTest.Parts
{
    [Export(typeof(ISharedInterface))]
    [ExportMetadata("Key", "A")]
    [ExportMetadata("Weight", 25)]
    class SharedInterfaceA
        : ISharedInterface
    {
    }
}
