using CQRSlite.Events;

namespace idee5.EventStoreLite;

/// <summary>
/// Base class for CQRSlite events.
/// </summary>
public abstract class CqrsEvent : IEvent {
    /// <summary>
    /// Initializes a new instance of the event.
    /// </summary>
    /// <param name="id">The id.</param>
    protected CqrsEvent(Guid id) {
        Id = id;
    }
    #region Public Properties

    /// <summary>
    /// The ID of the aggregate being affected by this event.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The UTC time when this event occurred.
    /// </summary>
    public DateTimeOffset TimeStamp { get; set; }

    /// <summary>
    /// The version of the aggregate which results from this event.
    /// </summary>
    public int Version { get; set; }

    #endregion Public Properties
}
