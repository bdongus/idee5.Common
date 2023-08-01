using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace idee5.Common.Tests {
    [TestClass]
    public class AmbientServiceTests {
        private interface IFoo {
        }

        private class Foo : IFoo {
        }

        private class Foo2 : IFoo { }

        private class AmbientServiceNoDefault : AmbientService<IFoo> {
        }

        private class AmbientServiceWithDefault : AmbientService<IFoo> {
            protected override IFoo DefaultCreate() {
                return new Foo();
            }
        }

        [UnitTest, TestMethod]
        public void Instance_WhenNoDefaultCreateOrCreateSet_ShouldThrow() {
            var sut = new AmbientServiceNoDefault();
            Action instance = () => {
                var x = sut.Instance;
            };
            Assert.ThrowsException<Exception>(instance);
        }

        [UnitTest, TestMethod]
        public void Instance_WhenCreateDelegateSupplied_ShouldReturnInstance() {
            // Arrange
            var sut = new AmbientServiceNoDefault {
                Create = () => new Foo()
            };
            // Act
            IFoo instance = sut.Instance;

            // Asssert
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [UnitTest, TestMethod]
        public void Instance_WhenDefaultDelegateSupplied_ShouldReturnInstance() {
            // Arrange
            var sut = new AmbientServiceWithDefault();
            // Act
            IFoo instance = sut.Instance;
            //Assert
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [UnitTest, TestMethod]
        public void Instance_WhenDefaultDelegateSuppliedAndCreateSet_ShouldReturnCreateInstance() {
            // Arrange
            var sut = new AmbientServiceWithDefault {
                Create = () => new Foo2()
            };
            // Act
            IFoo instance = sut.Instance;
            // Asert
            Assert.IsInstanceOfType(instance, typeof(Foo2));
        }

        [UnitTest, TestMethod]
        public void Instance_WhenInstanceSet_ShouldReturnInstance() {
            // Arrange
            var sut = new AmbientServiceWithDefault {
                Create = () => new Foo2(),
                Instance = new Foo()
            };
            // Act
            var instance = sut.Instance;
            // Assert
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [UnitTest, TestMethod]
        public void Instance_WhenInstanceSetToNull_ShouldThrow() {
            // Arrange
            var sut = new AmbientServiceWithDefault {
                Create = () => new Foo2()
            };
            // Act
            Action setInstanceToNull = () => sut.Instance = null;
            // Assert
            Assert.ThrowsException<ArgumentNullException>(setInstanceToNull);
        }
    }
}