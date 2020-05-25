namespace idee5.Common {
    /// <summary>
    /// Command handler interface.
    /// </summary>
    /// <remarks>https://www.cuttingedge.it/blogs/steven/pivot/entry.php?id=91</remarks>
    /// <typeparam name="TCommand">Command parameter(s).</typeparam>
    public interface ICommandHandler<TCommand> {
        /// <summary>
        /// Handle/execute the command.
        /// </summary>
        /// <param name="command">The command parameter(s).</param>
        void Handle(TCommand command);
    }
}