namespace idee5.Common.Data {
    /// <summary>
    /// Defines interface for base entity type. All entities in the system must implement this interface.
    /// </summary>
    /// <typeparam name="TPrimaryKey">Type of the primary key of the entity</typeparam>
    public interface IEntity<TPrimaryKey> where TPrimaryKey : notnull {
        /// <summary>
        /// Unique identifier for this instance.
        /// </summary>
        TPrimaryKey Id { get; }
    }
}