using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace idee5.Common_461Tests {
    [System.AttributeUsageAttribute(System.AttributeTargets.Method, AllowMultiple = false)]
    /// <summary>
    /// Use this category to denote unit tests. The Continuous Integration build can use this
    /// category to run only these tests.
    /// </summary>
    public class UnitTestAttribute : TestCategoryBaseAttribute {
        public override IList<string> TestCategories {
            get { return new List<string> { "UnitTest" }; }
        }
    }
}