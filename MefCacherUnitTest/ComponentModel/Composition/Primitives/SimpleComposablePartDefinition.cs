using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhNoPub.MefCacherUnitTest.ComponentModel.Composition.Primitives
{
    /// <summary>
    ///   A simple <see cref="ComposablePartDefinition"/> (no metadata).
    /// </summary>
    class SimpleComposablePartDefinition
        : ComposablePartDefinition
    {
        public override IEnumerable<ExportDefinition> ExportDefinitions => new[]
        {
            new ExportDefinition(
                "X",
                new Dictionary<string, object>
                {
                    { "Weight", 0 },
                }),
        };

        public override IEnumerable<ImportDefinition> ImportDefinitions => new ImportDefinition[]
        {/*
            new ImportDefinition(
                "Z",

                new Dictionary<string, object>
                {
                })*/
        };

        public override ComposablePart CreatePart()
        {
            throw new NotImplementedException();
        }
    }
}
