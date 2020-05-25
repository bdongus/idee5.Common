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
#pragma warning disable HAA0302 // Display class allocation to capture closure
            Exception exception = null;
#pragma warning restore HAA0302 // Display class allocation to capture closure

            // Act
#pragma warning disable HAA0301 // Closure Allocation Source
#pragma warning disable IDE0039 // Lokale Funktion verwenden
            Action<Exception> onException = ex => exception = ex;
#pragma warning restore IDE0039 // Lokale Funktion verwenden
#pragma warning restore HAA0301 // Closure Allocation Source
            NoParameterDelayedExceptionTask().SafeFireAndForget(onException: onException);
            await Task.Delay(500).ConfigureAwait(false);
            await Task.Delay(500).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(exception);
        }
    }
}
