namespace idee5.Common {
    /// <summary>
    /// Interface for providing the current user's id.
    /// </summary>
    public interface ICurrentUserIdProvider {
        /// <summary>
        /// Get the id of the current user.
        /// </summary>
        string GetCurrentUserId();
    }
}