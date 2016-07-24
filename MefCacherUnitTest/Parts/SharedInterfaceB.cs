using System.ComponentModel.Composition;

namespace OhNoPub.MefCacherUnitTest.Parts
{
    [Export(typeof(ISharedInterface))]
    [ExportMetadata("Key", "B")]
    [ExportMetadata("Weight", -1)]
    class SharedInterfaceB
        : ISharedInterface
    {
    }
}
