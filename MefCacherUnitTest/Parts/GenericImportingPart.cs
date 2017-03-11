using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhNoPub.MefCacherUnitTest.Parts
{
    [Export]
    class GenericImportingPart
    {
        [Import]
        public IGenericContract<int, long> GenericThing { get; set; }
    }
}
