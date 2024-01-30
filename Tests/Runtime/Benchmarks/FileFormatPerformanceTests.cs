using EventHorizon.FileFormat;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace EventHorizon.Tests.Benchmarks
{
	public class FileFormatPerformanceTests
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
			var jsonPacketData = PacketUtils.SerializePacket(metadata);

			Measure.Method(() =>
				{
					PacketUtils.DeserializePacket(jsonPacketData.header, jsonPacketData.contents);
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
			var jsonPacketData = PacketUtils.SerializePacket(transformPacket);

			Measure.Method(() =>
				{
					PacketUtils.DeserializePacket(jsonPacketData.header, jsonPacketData.contents);
				})
				.WarmupCount(100)
				.MeasurementCount(1000)
				.SampleGroup(new SampleGroup("TransformPacket Deserialization", SampleUnit.Microsecond))
				.Run();
		}

		#endregion
		
		#region Frame Packet
		
		[Test]
		[Performance]
		public void FramePacket_Construction() =>
			Measure.Method(() =>
				{
					_ = new FramePacket();
				})
				.WarmupCount(100)
				.MeasurementCount(1000)
				.SampleGroup(new SampleGroup("FramePacket Constructor", SampleUnit.Microsecond))
				.Run();


		[Test]
		[Performance]
		public void FramePacket_Serialization()
		{
			var framePacket = new FramePacket();

			Measure.Method(() =>
				{
					PacketUtils.SerializePacket(framePacket);
				})
				.WarmupCount(100)
				.MeasurementCount(1000)
				.SampleGroup(new SampleGroup("FramePacket Serialization", SampleUnit.Microsecond))
				.Run();
		}

		[Test]
		[Performance]
		public void FramePacket_Deserialization()
		{
			var framePacket = new FramePacket();
			var jsonPacketData = PacketUtils.SerializePacket(framePacket);

			Measure.Method(() =>
				{
					PacketUtils.DeserializePacket(jsonPacketData.header, jsonPacketData.contents);
				})
				.WarmupCount(100)
				.MeasurementCount(1000)
				.SampleGroup(new SampleGroup("FramePacket Deserialization", SampleUnit.Microsecond))
				.Run();
		}

		#endregion
		
		#region Generic Data Packet

		[Test]
		[Performance]
		public void GenericDataPacket_Construction() =>
			Measure.Method(() =>
				{
					_ = new GenericDataPacket();
				})
				.WarmupCount(100)
				.MeasurementCount(1000)
				.SampleGroup(new SampleGroup("GenericDataPacket Constructor", SampleUnit.Microsecond))
				.Run();


		[Test]
		[Performance]
		public void GenericDataPacket_Serialization()
		{
			var genericDataPacket = new GenericDataPacket();

			Measure.Method(() =>
				{
					PacketUtils.SerializePacket(genericDataPacket);
				})
				.WarmupCount(100)
				.MeasurementCount(1000)
				.SampleGroup(new SampleGroup("GenericDataPacket Serialization", SampleUnit.Microsecond))
				.Run();
		}

		[Test]
		[Performance]
		public void GenericDataPacket_Deserialization()
		{
			var genericDataPacket = new GenericDataPacket();
			var jsonPacketData = PacketUtils.SerializePacket(genericDataPacket);

			Measure.Method(() =>
				{
					PacketUtils.DeserializePacket(jsonPacketData.header, jsonPacketData.contents);
				})
				.WarmupCount(100)
				.MeasurementCount(1000)
				.SampleGroup(new SampleGroup("GenericDataPacket Deserialization", SampleUnit.Microsecond))
				.Run();
		}

		#endregion
		
		#region Activation Packet

		[Test]
		[Performance]
		public void ActivationPacket_Construction() =>
			Measure.Method(() =>
				{
					_ = new ActivationPacket();
				})
				.WarmupCount(100)
				.MeasurementCount(1000)
				.SampleGroup(new SampleGroup("ActivationPacket Constructor", SampleUnit.Microsecond))
				.Run();


		[Test]
		[Performance]
		public void ActivationPacket_Serialization()
		{
			var activationPacket = new ActivationPacket();

			Measure.Method(() =>
				{
					PacketUtils.SerializePacket(activationPacket);
				})
				.WarmupCount(100)
				.MeasurementCount(1000)
				.SampleGroup(new SampleGroup("ActivationPacket Serialization", SampleUnit.Microsecond))
				.Run();
		}

		[Test]
		[Performance]
		public void ActivationPacket_Deserialization()
		{
			var activationPacket = new ActivationPacket();
			var jsonPacketData = PacketUtils.SerializePacket(activationPacket);

			Measure.Method(() =>
				{
					PacketUtils.DeserializePacket(jsonPacketData.header, jsonPacketData.contents);
				})
				.WarmupCount(100)
				.MeasurementCount(1000)
				.SampleGroup(new SampleGroup("ActivationPacket Deserialization", SampleUnit.Microsecond))
				.Run();
		}

		#endregion
		
		#region VRMetadata Packet

		[Test]
		[Performance]
		public void VRMetadataPacket_Construction() =>
			Measure.Method(() =>
				{
					_ = new VRMetadataPacket();
				})
				.WarmupCount(100)
				.MeasurementCount(1000)
				.SampleGroup(new SampleGroup("VRMetadataPacket Constructor", SampleUnit.Microsecond))
				.Run();


		[Test]
		[Performance]
		public void VRMetadataPacket_Serialization()
		{
			var vrMetadataPacket = new VRMetadataPacket();

			Measure.Method(() =>
				{
					PacketUtils.SerializePacket(vrMetadataPacket);
				})
				.WarmupCount(100)
				.MeasurementCount(1000)
				.SampleGroup(new SampleGroup("VRMetadataPacket Serialization", SampleUnit.Microsecond))
				.Run();
		}

		[Test]
		[Performance]
		public void VRMetadataPacket_Deserialization()
		{
			var vrMetadataPacket = new VRMetadataPacket();
			var jsonPacketData = PacketUtils.SerializePacket(vrMetadataPacket);

			Measure.Method(() =>
				{
					PacketUtils.DeserializePacket(jsonPacketData.header, jsonPacketData.contents);
				})
				.WarmupCount(100)
				.MeasurementCount(1000)
				.SampleGroup(new SampleGroup("VRMetadataPacket Deserialization", SampleUnit.Microsecond))
				.Run();
		}

		#endregion

	}
}