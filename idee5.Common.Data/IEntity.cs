﻿namespace idee5.Common.Data;
/// <summary>
/// A shortcut of <see cref="IEntity{TPrimaryKey}"/> for most used primary key type (<see cref="int"/>).
/// </summary>
public interface IEntity : IEntity<int> {
}