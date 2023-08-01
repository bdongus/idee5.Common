using System;

namespace idee5.Common.Data {
    /// <summary>
    /// A shortcut of <see cref="IEntity{TPrimaryKey}"/> for often used primary key type (<see cref="Guid"/>).
    /// </summary>
    public interface IGuidEntity : IEntity<Guid> {
    }
}