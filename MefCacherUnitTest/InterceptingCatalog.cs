using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;

namespace OhNoPub.MefCacherUnitTest
{
    class InterceptingCatalog : ComposablePartCatalog
    {
        /// <summary>
        ///   Changes whenever an enumeration happens.
        /// </summary>
        public object EnumerationToken { get; private set; } = new object();

        Func<IEnumerator<ComposablePartDefinition>> Enumerate { get; }

        public InterceptingCatalog(
            ComposablePartCatalog catalog)
            : this(catalog.GetEnumerator)
        {
        }

        public InterceptingCatalog(
            Func<IEnumerator<ComposablePartDefinition>> enumerate)
        {
            Enumerate = enumerate;
        }

        public override IEnumerator<ComposablePartDefinition> GetEnumerator()
        {
            EnumerationToken = new object();
            return Enumerate();
        }
    }
}
