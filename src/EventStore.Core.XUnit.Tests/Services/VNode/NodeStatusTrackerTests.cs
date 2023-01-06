﻿using System;
using System.Diagnostics.Metrics;
using System.Linq;
using EventStore.Core.Services.VNode;
using EventStore.Core.Telemetry;
using EventStore.Core.XUnit.Tests.Telemetry;
using Xunit;

namespace EventStore.Core.TransactionLog.Services.VNode {
	public class NodeStatusTrackerTests : IDisposable {
		private readonly TestMeterListener<long> _listener;
		private readonly FakeClock _clock = new();
		private readonly StatusMetric _metric;
		private readonly NodeStatusTracker _sut;

		public NodeStatusTrackerTests() {
			var meter = new Meter($"{typeof(NodeStatusTrackerTests)}");
			_listener = new TestMeterListener<long>(meter);
			_metric = new StatusMetric(
				meter,
				"eventstore-statuses",
				_clock);
			_sut = new NodeStatusTracker(_metric);
		}

		public void Dispose() {
			_listener?.Dispose();
		}

		[Fact]
		public void can_observe_state_change() {
			_clock.SecondsSinceEpoch = 500;
			AssertMeasurements("Initializing", 500);

			_sut.OnStateChange(Data.VNodeState.PreReplica);
			_clock.SecondsSinceEpoch = 501;
			AssertMeasurements("PreReplica", 501);

			_sut.OnStateChange(Data.VNodeState.Follower);
			_clock.SecondsSinceEpoch = 502;
			AssertMeasurements("Follower", 502);
		}

		void AssertMeasurements(string expectedStatus, int expectedValue) {
			_listener.Observe();

			Assert.Collection(
				_listener.RetrieveMeasurements("eventstore-statuses"),
				m => {
					Assert.Equal(expectedValue, m.Value);
					Assert.Collection(
						m.Tags.ToArray(),
						t => {
							Assert.Equal("name", t.Key);
							Assert.Equal("Node", t.Value);
						},
						t => {
							Assert.Equal("status", t.Key);
							Assert.Equal(expectedStatus, t.Value);
						});
				});
		}
	}
}
