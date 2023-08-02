using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Tests {
    public class AsyncQueryParameters : IQuery<string[]> {
        public string Searchtext { get; set; }
    }

    public class AsyncQueryHandler : IQueryHandlerAsync<AsyncQueryParameters, string[]> {
        /// <summary>
        /// Handles the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public Task<string[]> HandleAsync(AsyncQueryParameters query, CancellationToken cancellationToken) {
            return Task.Run(() => query.Searchtext.Split(separator: new char[] { ' ' }));
        }
    }

    [TestClass]
    public class AsyncQueryHandlerTests {
        [UnitTest, TestMethod]
        public async Task TestAsyncQueryHandler() {
            var q = new AsyncQueryParameters { Searchtext = "bli bla blubb" };
            var handler = new AsyncQueryHandler();
            string[] s = await handler.HandleAsync(q, new CancellationToken()).ConfigureAwait(false);

            Assert.AreEqual(expected: 3, actual: s.Length);

            Assert.AreEqual(expected: "bla", actual: s[1]);
        }
    }
}