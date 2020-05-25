using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace idee5.Common.Tests {
    [TestClass]
    public class GuidExtensionTests {
        [TestMethod, UnitTest]
        public void Test_KnownGUID_A() {
            // Arrange

            // Act
            var test = new Guid("6478c9bd-2157-11df-8f70-001c234418a3");

            // Assert

            // This GUID was generated on 2010-02-24 on Steven Cleary's machine using "uuidgen -x"
            Assert.AreEqual(GuidVariant.RFC4122, test.GetVariant());
            Assert.AreEqual(GuidVersion.TimeBased, test.GetVersion());
            Assert.IsTrue(test.NodeIsMAC());
            Assert.IsTrue(test.GetNode().SequenceEqual(new byte[] { 0x00, 0x1C, 0x23, 0x44, 0x18, 0xA3 })); // This MAC address was retrieved using "ipconfig /all"
            Assert.IsTrue(test.GetCreateTime().Date == new DateTime(2010, 02, 24));
        }

        [TestMethod, UnitTest]
        public void Test_KnownGUID_B() {
            // Arrange

            // Act
            var test = new Guid("661a0750-2157-11df-8f70-001c234418a3");

            // Assert

            // This GUID was generated on 2010-02-24 on Steven Cleary's machine using "uuidgen -x"
            Assert.AreEqual(GuidVariant.RFC4122, test.GetVariant());
            Assert.AreEqual(GuidVersion.TimeBased, test.GetVersion());
            Assert.IsTrue(test.NodeIsMAC());
            Assert.IsTrue(test.GetNode().SequenceEqual(new byte[] { 0x00, 0x1C, 0x23, 0x44, 0x18, 0xA3 })); // This MAC address was retrieved using "ipconfig /all"
            Assert.IsTrue(test.GetCreateTime().Date == new DateTime(2010, 02, 24));
        }

        [TestMethod, UnitTest]
        public void Test_KnownGUID_C() {
            // Arrange

            // Act
            var test = new Guid("87ce17d1-7d63-42e5-aafb-4e105942fbf7");

            // Assert

            // This GUID was generated on 2010-02-24 on Steven Cleary's machine using "uuidgen"
            Assert.AreEqual(GuidVariant.RFC4122, test.GetVariant());
            Assert.AreEqual(GuidVersion.Random, test.GetVersion());
        }
    }
}
