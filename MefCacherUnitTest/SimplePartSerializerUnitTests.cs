using Microsoft.VisualStudio.TestTools.UnitTesting;
using OhNoPub.MefCacherUnitTest.ComponentModel.Composition.Primitives;

namespace OhNoPub.MefCacherUnitTest
{
    [TestClass]
    public class SimplePartSerializerUnitTests
    {
        [TestMethod]
        public void RoundTripSimpleComposablePartDefinition()
        {
            var definition = new SimpleComposablePartDefinition();
        }
    }
}
