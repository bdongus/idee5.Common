namespace idee5.Common {
    /// <inheritdoc />
    public class DefaultCurrentUserIdProvider : ICurrentUserIdProvider {
        /// <inheritdoc />
        public string GetCurrentUserId() => System.Threading.Thread.CurrentPrincipal?.Identity?.Name ?? "anonymous";
    }
}