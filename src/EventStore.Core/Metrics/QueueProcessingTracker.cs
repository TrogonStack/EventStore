using System.Collections.Generic;
using EventStore.Core.Time;

namespace EventStore.Core.Metrics {
	public interface IQueueProcessingTracker {
		Instant RecordNow(Instant start, string messageType);
	}

	public class QueueProcessingTracker : IQueueProcessingTracker {
		private readonly DurationMetric _metric;
		private readonly string _queueName;

		public QueueProcessingTracker(DurationMetric metric, string queueName) {
			_metric = metric;
			_queueName = queueName;
		}

		public Instant RecordNow(Instant start, string messageType) {
			return _metric.Record(
				start: start,
				new KeyValuePair<string, object>("queue", _queueName),
				new KeyValuePair<string, object>("message-type", messageType));
		}

		public class NoOp : IQueueProcessingTracker {
			public Instant RecordNow(Instant start, string messageType) => start;
		}
	}
}
