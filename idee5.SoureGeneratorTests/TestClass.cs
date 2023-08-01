using idee5.Common;
using System;
using System.Collections.Generic;

namespace idee5.SoureGeneratorTests {
    [GenerateCommands("HandlerTemplate.txt", PropertyTemplate = "NoDocProperty.txt")]
    internal class TestClass {
        private readonly List<DateTime> _datetimes;

        public int Prop1 { get; }
        public int ProtectedProp { get; protected set; }
        public IReadOnlyList<DateTime> Deployments => _datetimes.AsReadOnly();

        /// <summary>
        /// Gimme 42
        /// </summary>
        /// <param name="para1"></param>
        /// <param name="para2">42 <see cref="StackOverflowException"/></param>
        public void TestMethod(int para1, DateTimeRange para2) { }
        public void ParamerlessMethod() { }
        protected void ProtectedMethod(string p1) { }
        private void PrivateMethod() { }
        public TestClass(List<DateTime> datetimes) {
            _datetimes = datetimes;
        }

    }
}