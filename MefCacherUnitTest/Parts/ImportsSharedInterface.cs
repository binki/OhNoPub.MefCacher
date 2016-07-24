using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace OhNoPub.MefCacherUnitTest.Parts
{
    [Export]
    class ImportsSharedInterface
    {
        [ImportMany]
        public IEnumerable<Lazy<ISharedInterface, ISharedInterfaceMetadata>> Things { get; set; }
    }
}
