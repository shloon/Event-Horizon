using EventHorizon.FormatV2;
using NUnit.Framework;
using System;
using System.Globalization;
using UnityEngine;

namespace EventHorizon.Tests
{
	public class FormatPacketSerializationTest
	{
		[Test]
		public void MetadataPacket_RoundTrip()
		{
			var metadataPacket = new MetadataPacket
			{
				version = RecordingFormatVersion.V2,
				fps = new FrameRate(24000, 1001),
				sceneName = "TestScene",
				timestamp = DateTime.Now.ToString(
					"yyyy-MM-dd'T'HH:mm:ss.fffK",
					CultureInfo.InvariantCulture)
			};

			var serializedPacket = PacketUtils.SerializePacket(metadataPacket);
			var deserializedPacket = PacketUtils.DeserializePacket(serializedPacket.header, serializedPacket.contents);

			Debug.Log(serializedPacket.contents);

			Assert.AreEqual(PacketType.Metadata, deserializedPacket.Type);
			Assert.AreEqual(metadataPacket, (MetadataPacket) deserializedPacket);
		}

		[Test]
		public void TransformPacket_RoundTrip()
		{
			var transformPacket = new TransformPacket
			{
				frame = 0,
				id = default,
				translation = new Vector3(1, 2, 3),
				rotation = new Quaternion(4, 5, 6, 7),
				scale = new Vector3(8, 9, 0)
			};

			var serializedPacket = PacketUtils.SerializePacket(transformPacket);
			var deserializedPacket = PacketUtils.DeserializePacket(serializedPacket.header, serializedPacket.contents);

			Debug.Log(serializedPacket.contents);

			Assert.AreEqual(PacketType.Transform, deserializedPacket.Type);
			Assert.AreEqual(transformPacket, (TransformPacket) deserializedPacket);
		}

		[Test]
		public void FrameDataPacket_RoundTrip()
		{
			var framePacket = new FramePacket { frame = 123, elapsedTime = 123 * 60.0 / 1000.0 };

			var serializedPacket = PacketUtils.SerializePacket(framePacket);
			var deserializedPacket = PacketUtils.DeserializePacket(serializedPacket.header, serializedPacket.contents);

			Debug.Log(serializedPacket.contents);

			Assert.AreEqual(PacketType.Frame, deserializedPacket.Type);
			Assert.AreEqual(framePacket, (FramePacket) deserializedPacket);
		}

		[Test]
		public void GenericDataPacket_RoundTrip()
		{
			var genericDataPacket = new GenericDataPacket { data = "Test" };

			var serializedPacket = PacketUtils.SerializePacket(genericDataPacket);
			var deserializedPacket = PacketUtils.DeserializePacket(serializedPacket.header, serializedPacket.contents);

			Debug.Log(serializedPacket.contents);

			Assert.AreEqual(PacketType.Generic, deserializedPacket.Type);
			Assert.AreEqual(genericDataPacket, (GenericDataPacket) deserializedPacket);
		}
	}
}