﻿using System;

namespace idee5.Common;
/// <summary>
/// <see cref="ICurrentUserIdProvider"/> wrapped in an ambient context.
/// </summary>
/// <example>public readonly AmbientUserProvider Provider = new AmbientUserProvider();</example>
[Obsolete("Not meant for use in production code!")]
public class AmbientUserProvider : AmbientService<ICurrentUserIdProvider>, ICurrentUserIdProvider {
    /// <inheritdoc />
    protected override ICurrentUserIdProvider DefaultCreate() => new DefaultCurrentUserIdProvider();

    /// <inheritdoc />
    public string GetCurrentUserId() => Instance.GetCurrentUserId();
}