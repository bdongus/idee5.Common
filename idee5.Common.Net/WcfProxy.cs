using System;
using System.ServiceModel;

namespace idee5.Common.Net {
    /// <summary>
    /// Generic class generating a WCF service proxy and handling exceptions.
    /// </summary>
    public static class WcfProxy {
        // Visual C# 2012 Praxisbuch Seite 510 ff.
        /// <summary>
        /// Execute the specified lambda body.
        /// </summary>
        /// <typeparam name="T">The service client type. No static types.</typeparam>
        /// <param name="body">The lambda expression body.</param>
        public static void Action<T>(Action<T> body) where T : ICommunicationObject, new()
        {
            var proxy = new T();
            ICommunicationObject comm = proxy;
            proxy.Open();
            // try to execute the function body
            try {
                body.Invoke(proxy);
                comm.Close();
            }
            catch (Exception) {
                // Close the proxy if possible, otherwise abort it.
                try {
                    if (comm.State == CommunicationState.Faulted)
                        comm.Abort();
                    else
                        comm.Close();
                }
                catch (Exception) { comm.Abort(); }
                // rethrow the exception
                throw;
            }
        }

        /// <summary>
        /// Execute the specified lambda body and return a result.
        /// </summary>
        /// <typeparam name="T">The type of the T. No static type</typeparam>
        /// <typeparam name="Result">The type of the result.</typeparam>
        /// <param name="body">The lambda expression body.</param>
        /// <returns>The result of the operation.</returns>
        public static Result Func<T, Result>(Func<T, Result> body) where T : ICommunicationObject, new()
        {
            var proxy = new T();
            ICommunicationObject comm = proxy;
            proxy.Open();
            // try to execute the function body
            try {
                Result retVal = body.Invoke(proxy);
                comm.Close();
                return retVal;
            }
            catch (Exception) {
                // Close the proxy if possible, otherwise abort it.
                try {
                    if (comm.State == CommunicationState.Faulted)
                        comm.Abort();
                    else
                        comm.Close();
                }
                catch (Exception) { comm.Abort(); }
                // rethrow the exception
                throw;
            }
        }
    }
}
