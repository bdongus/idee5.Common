using CQRSlite.Events;
namespace idee5.EventstorLiteTests;

public class TestEventPublisher : IEventPublisher {
    #region Public Properties

    public int Published { get; private set; }

    #endregion Public Properties

    #region Public Methods

    public Task Publish<T>(T @event, CancellationToken cancellationToken = default) where T : class, IEvent {
        Published++;
        return Task.CompletedTask;
    }

    #endregion Public Methods
}
