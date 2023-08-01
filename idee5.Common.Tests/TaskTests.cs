using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace idee5.Common.Tests {
    [TestClass]
    public class TaskTests {
        protected async Task NoParameterDelayedExceptionTask() {
            await Task.Delay(500).ConfigureAwait(false);
            throw new Exception();
        }

        [TestMethod]
        public async Task CanCatchFireAndForgetException() {
            // Arrange
            Exception exception = null;

            // Act
#pragma warning disable IDE0039 // Lokale Funktion verwenden
            Action<Exception> onException = ex => exception = ex;
#pragma warning restore IDE0039 // Lokale Funktion verwenden
            NoParameterDelayedExceptionTask().SafeFireAndForget(onException: onException);
            await Task.Delay(500).ConfigureAwait(false);
            await Task.Delay(500).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(exception);
        }
    }
}
