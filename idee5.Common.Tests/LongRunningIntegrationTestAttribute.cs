using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace idee5.Common.Tests {
    /// <summary>
    /// Use this category to denote integration tests that you might not want to include in typical
    /// builds because the tests take a long time to complete.
    /// </summary>
    public class LongRunningIntegrationTestAttribute : TestCategoryBaseAttribute {
        public override IList<string> TestCategories {
            get { return new List<string> { "LongRunningIntegrationTest" }; }
        }
    }
}