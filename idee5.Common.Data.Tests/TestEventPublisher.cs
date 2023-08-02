using CQRSlite.Events;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Common.Data.Tests {
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
}
