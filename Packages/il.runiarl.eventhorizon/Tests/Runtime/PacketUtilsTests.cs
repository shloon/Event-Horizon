using EventHorizon.FileFormat;
using NUnit.Framework;
using System;

namespace EventHorizon.Tests
{
	[Parallelizable]
	public class PacketUtilsTests
	{
		[Test]
		public void GenerateHeader_GeneratesCorrectHeaders()
		{
			Assert.AreEqual(new PacketHeader() { type = PacketType.Activation}, PacketUtils.GenerateHeader(new ActivationPacket()));
			Assert.AreEqual(new PacketHeader() { type = PacketType.Generic}, PacketUtils.GenerateHeader(new GenericDataPacket()));
			Assert.AreEqual(new PacketHeader() { type = PacketType.Frame}, PacketUtils.GenerateHeader(new FramePacket()));
			Assert.AreEqual(new PacketHeader() { type = PacketType.VRMetadata}, PacketUtils.GenerateHeader(new VRMetadataPacket()));
			Assert.AreEqual(new PacketHeader() { type = PacketType.Metadata}, PacketUtils.GenerateHeader(new MetadataPacket()));
			Assert.AreEqual(new PacketHeader() { type = PacketType.Transform}, PacketUtils.GenerateHeader(new TransformPacket()));
		}
		
		[Test]
		public void GenerateMetadataPacket_CreatesPacketInCorrectStructure()
		{
			var sceneName = "TestScene";
			var fps = new FrameRate(24000, 1001);
			var time = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

			var packet = PacketUtils.GenerateMetadataPacket(sceneName, fps, time);
			
			Assert.AreEqual(RecordingFormatVersion.V1,packet.version);
			Assert.AreEqual(sceneName, packet.sceneName);
			Assert.AreEqual(fps, packet.fps);
			Assert.AreEqual("1970-01-01T00:00:00.000Z", packet.timestamp);
		}
	}
}