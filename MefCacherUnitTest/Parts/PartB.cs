using System.ComponentModel.Composition;

namespace OhNoPub.MefCacherUnitTest.Parts
{
    [Export]
    class PartB
    {
        [Import]
        public PartA PartA { get; set; }
    }
}
