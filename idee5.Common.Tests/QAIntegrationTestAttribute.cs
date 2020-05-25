using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace idee5.Common.Tests {
    /// <summary>
    /// Use this category to denote integration tests that will be run when the services are deployed
    /// to the Quality Assurance (QA) environment.
    /// </summary>
    public class QAIntegrationTestAttribute : TestCategoryBaseAttribute {
        public override IList<string> TestCategories {
            get { return new List<string> { "QAIntegrationTest" }; }
        }
    }
}