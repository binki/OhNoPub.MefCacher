using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace OhNoPub.MefCacherUnitTest.Parts
{
    [Export]
    class ImportsManyNonexistent
    {
        [ImportMany]
        public IEnumerable<NonExportedPart> ShouldBeEmpty { get; set; }

        public class NonExportedPart { }
    }
}
