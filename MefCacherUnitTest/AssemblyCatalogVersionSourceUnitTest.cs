using Microsoft.VisualStudio.TestTools.UnitTesting;
using OhNoPub.MefCacher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhNoPub.MefCacherUnitTest
{
    [TestClass]
    public class AssemblyCatalogVersionSourceUnitTest
    {
        [TestMethod]
        public async Task GetVersion()
        {
            // Basically we want it to be able to give different values when the file
            // changes modification date or length but return the same thing when
            // queried multiple times if neither of those has changed.
            var fakeAssemblyFileName = "myFakeAssembly.dll";
            var source = new AssemblyCatalogVersionSource(fakeAssemblyFileName);
            File.WriteAllText(fakeAssemblyFileName, "a");
            var nextTimeStampBarrier = Task.Delay(TimeSpan.FromSeconds(2));

            var initialVersion = source.GetVersion(null);
            Console.WriteLine(initialVersion);
            Assert.IsNotNull(initialVersion);
            Assert.AreEqual(initialVersion, source.GetVersion(null));

            // Should change when timestamp changed
            await nextTimeStampBarrier; // Wait long enough for clock to change
            File.WriteAllText(fakeAssemblyFileName, "a");
            var nextVersion = source.GetVersion(null);
            Console.WriteLine(nextVersion);
            Assert.IsNotNull(nextVersion);
            Assert.AreNotEqual(initialVersion, nextVersion);
            Assert.AreEqual(nextVersion, source.GetVersion(null));

            // Clock may or may not have ticked by now. It probably has. But just
            // verify that size change causes difference.
            File.WriteAllText(fakeAssemblyFileName, "ab");
            var lastVersion = source.GetVersion(null);
            Console.WriteLine(lastVersion);
            Assert.IsNotNull(lastVersion);
            Assert.AreNotEqual(initialVersion, lastVersion);
            Assert.AreNotEqual(nextTimeStampBarrier, lastVersion);
            Assert.AreEqual(lastVersion, source.GetVersion(null));
        }
    }
}
