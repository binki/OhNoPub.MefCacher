using Microsoft.VisualStudio.TestTools.UnitTesting;
using OhNoPub.MefCacher;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhNoPub.MefCacherUnitTest
{
    [TestClass]
    public class ConstantCatalogVersionSourceUnitTests
    {
        [TestMethod]
        public void GetVersion()
        {
            foreach (var version in new[] { null, "", "asdf", })
                Assert.AreEqual(
                    version,
                    // Shouldn’t even dereference its parameter.
                    new ConstantCatalogVersionSource(version).GetVersion(null));
        }
    }
}
