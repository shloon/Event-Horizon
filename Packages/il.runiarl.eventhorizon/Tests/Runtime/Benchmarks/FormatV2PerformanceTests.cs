using EventHorizon.FormatV2;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace EventHorizon.Tests.Benchmarks
{
	public class FormatV2PerformanceTests
	{
		[Test]
		[Performance]
		public void PacketHeader_Construction() =>
			Measure.Method(() =>
				{
					_ = new PacketHeader();
				})
				.WarmupCount(100)
				.MeasurementCount(1000)
				.SampleGroup(new SampleGroup("PacketHeader Constructor", SampleUnit.Microsecond))
				.Run();

		[Test]
		[Performance]
		public void GenerateHeader()
		{
			var metadata = new MetadataPacket();

			Measure.Method(() =>
				{
					_ = PacketUtils.GenerateHeader(metadata);
				})
				.WarmupCount(100)
				.MeasurementCount(1000)
				.SampleGroup(new SampleGroup("GenerateHeader", SampleUnit.Microsecond))
				.Run();
		}

		#region Metadata Packet

		[Test]
		[Performance]
		public void MetadataPacket_Construction() =>
			Measure.Method(() =>
				{
					_ = new MetadataPacket();
				})
				.WarmupCount(100)
				.MeasurementCount(1000)
				.SampleGroup(new SampleGroup("MetadataPacket Constructor", SampleUnit.Microsecond))
				.Run();


		[Test]
		[Performance]
		public void MetadataPacket_Serialization()
		{
			var metadata = new MetadataPacket();

			Measure.Method(() =>
				{
					PacketUtils.SerializePacket(metadata);
				})
				.WarmupCount(100)
				.MeasurementCount(1000)
				.SampleGroup(new SampleGroup("MetadataPacket Serialization", SampleUnit.Microsecond))
				.Run();
		}

		[Test]
		[Performance]
		public void MetadataPacket_Deserialization()
		{
			var metadata = new MetadataPacket();
			var packet = PacketUtils.SerializePacket(metadata);

			Measure.Method(() =>
				{
					PacketUtils.DeserializePacket(packet.header, packet.contents);
				})
				.WarmupCount(100)
				.MeasurementCount(1000)
				.SampleGroup(new SampleGroup("MetadataPacket Deserialization", SampleUnit.Microsecond))
				.Run();
		}

		#endregion

		#region Transform Packet

		[Test]
		[Performance]
		public void TransformPacket_Construction() =>
			Measure.Method(() =>
				{
					_ = new TransformPacket();
				})
				.WarmupCount(100)
				.MeasurementCount(1000)
				.SampleGroup(new SampleGroup("TransformPacket Constructor", SampleUnit.Microsecond))
				.Run();


		[Test]
		[Performance]
		public void TransformPacket_Serialization()
		{
			var transformPacket = new TransformPacket();

			Measure.Method(() =>
				{
					PacketUtils.SerializePacket(transformPacket);
				})
				.WarmupCount(100)
				.MeasurementCount(1000)
				.SampleGroup(new SampleGroup("TransformPacket Serialization", SampleUnit.Microsecond))
				.Run();
		}

		[Test]
		[Performance]
		public void TransformPacket_Deserialization()
		{
			var transformPacket = new TransformPacket();
			var packet = PacketUtils.SerializePacket(transformPacket);

			Measure.Method(() =>
				{
					PacketUtils.DeserializePacket(packet.header, packet.contents);
				})
				.WarmupCount(100)
				.MeasurementCount(1000)
				.SampleGroup(new SampleGroup("TransformPacket Deserialization", SampleUnit.Microsecond))
				.Run();
		}

		#endregion
	}
}