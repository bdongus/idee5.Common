using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace idee5.Common.Tests {
    /// <summary>
    /// Use this category to denote integration tests that will run as part of the nightly build process.
    /// </summary>
    public class IntegrationTestAttribute : TestCategoryBaseAttribute {
        public override IList<string> TestCategories {
            get { return new List<string> { "IntegrationTest" }; }
        }
    }
}