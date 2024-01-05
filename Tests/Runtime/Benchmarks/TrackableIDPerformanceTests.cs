using NUnit.Framework;
using Unity.PerformanceTesting;

namespace EventHorizon.Tests.Benchmarks
{
	public class TrackableIDPerformanceTests
	{
		[Test]
		[Performance]
		public void ConstructorPerformanceTest()
		{
			Measure.Method(() =>
				{
					_ = new TrackableID(123);
				})
				.WarmupCount(10)
				.MeasurementCount(1000)
				.SampleGroup(new SampleGroup("TrackableID Constructor", SampleUnit.Microsecond))
				.Run();

#pragma warning disable CS0219
			Measure.Method(() =>
				{
					_ = 0;
				})
				.WarmupCount(10)
				.MeasurementCount(1000)
				.SampleGroup(new SampleGroup("UInt Constructor", SampleUnit.Microsecond))
				.Run();
#pragma warning restore CS0219
		}

		[Test]
		[Performance]
		public void EqualityPerformanceTest()
		{
			var trackableId1 = new TrackableID(123);
			var trackableId2 = new TrackableID(123);
			uint uint1 = 123;
			uint uint2 = 123;

			Measure.Method(() =>
				{
					_ = trackableId1.Equals(trackableId2);
				})
				.WarmupCount(10)
				.MeasurementCount(1000)
				.SampleGroup(new SampleGroup("TrackableID Equality Check", SampleUnit.Microsecond))
				.Run();

			Measure.Method(() =>
				{
					_ = uint1 == uint2;
				})
				.WarmupCount(10)
				.MeasurementCount(1000)
				.SampleGroup(new SampleGroup("UInt Equality Check", SampleUnit.Microsecond))
				.Run();
		}

		[Test]
		[Performance]
		public void GetHashCodePerformanceTest()
		{
			var trackableId = new TrackableID(123);
			uint uintId = 123;

			Measure.Method(() =>
				{
					_ = trackableId.GetHashCode();
				})
				.WarmupCount(10)
				.MeasurementCount(1000)
				.SampleGroup(new SampleGroup("TrackableID GetHashCode", SampleUnit.Microsecond))
				.Run();

			Measure.Method(() =>
				{
					_ = uintId.GetHashCode();
				})
				.WarmupCount(10)
				.MeasurementCount(1000)
				.SampleGroup(new SampleGroup("UInt GetHashCode", SampleUnit.Microsecond))
				.Run();
		}

		[Test]
		[Performance]
		public void ToStringPerformanceTest()
		{
			var trackableId = new TrackableID(123);
			uint uintId = 123;

			Measure.Method(() =>
				{
					_ = trackableId.ToString();
				})
				.WarmupCount(10)
				.MeasurementCount(1000)
				.SampleGroup(new SampleGroup("TrackableID ToString", SampleUnit.Microsecond))
				.Run();

			Measure.Method(() =>
				{
					_ = uintId.ToString();
				})
				.WarmupCount(10)
				.MeasurementCount(1000)
				.SampleGroup(new SampleGroup("UInt ToString", SampleUnit.Microsecond))
				.Run();
		}
	}
}