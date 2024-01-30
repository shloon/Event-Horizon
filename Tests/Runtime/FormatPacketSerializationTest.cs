using EventHorizon.FileFormat;
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
				version = RecordingFormatVersion.V1,
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
		public void MetadataPacket_DeSer_Equivalence()
		{
			var metadataPacket = new MetadataPacket{};
			var serializedExpected = JsonUtility.ToJson(metadataPacket);
			var deserializedExpected = JsonUtility.FromJson<MetadataPacket>(serializedExpected);

			var serializedActual = PacketUtils.SerializePacket(metadataPacket);
			var deserializedActual = PacketUtils.DeserializePacket(serializedActual.header, serializedActual.contents);
			
			Assert.AreEqual(serializedExpected, serializedActual.contents);
			Assert.AreEqual(deserializedExpected, deserializedActual);
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
		public void TransformPacket_DeSer_Equivalence()
		{
			var transformPacket = new TransformPacket{};
			var serializedExpected = JsonUtility.ToJson(transformPacket);
			var deserializedExpected = JsonUtility.FromJson<TransformPacket>(serializedExpected);

			var serializedActual = PacketUtils.SerializePacket(transformPacket);
			var deserializedActual = PacketUtils.DeserializePacket(serializedActual.header, serializedActual.contents);
			
			Assert.AreEqual(serializedExpected, serializedActual.contents);
			Assert.AreEqual(deserializedExpected, deserializedActual);
		}
		
		[Test]
		public void FramePacket_RoundTrip()
		{
			var framePacket = new FramePacket { frame = 123, elapsedTime = 123 * 60.0 / 1000.0 };

			var serializedPacket = PacketUtils.SerializePacket(framePacket);
			var deserializedPacket = PacketUtils.DeserializePacket(serializedPacket.header, serializedPacket.contents);

			Debug.Log(serializedPacket.contents);

			Assert.AreEqual(PacketType.Frame, deserializedPacket.Type);
			Assert.AreEqual(framePacket, (FramePacket) deserializedPacket);
		}
		
		[Test]
		public void FramePacket_DeSer_Equivalence()
		{
			var framePacket = new FramePacket{};
			var serializedExpected = JsonUtility.ToJson(framePacket);
			var deserializedExpected = JsonUtility.FromJson<FramePacket>(serializedExpected);

			var serializedActual = PacketUtils.SerializePacket(framePacket);
			var deserializedActual = PacketUtils.DeserializePacket(serializedActual.header, serializedActual.contents);
			
			Assert.AreEqual(serializedExpected, serializedActual.contents);
			Assert.AreEqual(deserializedExpected, deserializedActual);
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
		
		[Test]
		public void GenericDataPacket_DeSer_Equivalence()
		{
			var genericDataPacket = new GenericDataPacket{};
			var serializedExpected = JsonUtility.ToJson(genericDataPacket);
			var deserializedExpected = JsonUtility.FromJson<GenericDataPacket>(serializedExpected);

			var serializedActual = PacketUtils.SerializePacket(genericDataPacket);
			var deserializedActual = PacketUtils.DeserializePacket(serializedActual.header, serializedActual.contents);
			
			Assert.AreEqual(serializedExpected, serializedActual.contents);
			Assert.AreEqual(deserializedExpected, deserializedActual);
		}

		[Test]
		public void ActivationPacket_RoundTrip()
		{
			var activationPacket = new ActivationPacket { frame = 1u, id = 2u };

			var serializedPacket = PacketUtils.SerializePacket(activationPacket);
			var deserializedPacket = PacketUtils.DeserializePacket(serializedPacket.header, serializedPacket.contents);

			Debug.Log(serializedPacket.contents);

			Assert.AreEqual(PacketType.Activation, deserializedPacket.Type);
			Assert.AreEqual(activationPacket, (ActivationPacket) deserializedPacket);
		}
		
		[Test]
		public void ActivationPacket_DeSer_Equivalence()
		{
			var activationPacket = new ActivationPacket{};
			var serializedExpected = JsonUtility.ToJson(activationPacket);
			var deserializedExpected = JsonUtility.FromJson<ActivationPacket>(serializedExpected);

			var serializedActual = PacketUtils.SerializePacket(activationPacket);
			var deserializedActual = PacketUtils.DeserializePacket(serializedActual.header, serializedActual.contents);
			
			Assert.AreEqual(serializedExpected, serializedActual.contents);
			Assert.AreEqual(deserializedExpected, deserializedActual);
		}

		[Test]
		public void VRMetadataPacket_RoundTrip()
		{
			var vrMetadataPacket = new VRMetadataPacket { headsetType = "Unknown", interactionProfile = "Unknown" };

			var serializedPacket = PacketUtils.SerializePacket(vrMetadataPacket);
			var deserializedPacket = PacketUtils.DeserializePacket(serializedPacket.header, serializedPacket.contents);

			Debug.Log(serializedPacket.contents);

			Assert.AreEqual(PacketType.VRMetadata, deserializedPacket.Type);
			Assert.AreEqual(vrMetadataPacket, (VRMetadataPacket) deserializedPacket);
		}
		
		[Test]
		public void VRMetadataPacket_DeSer_Equivalence()
		{
			var vrMetadataPacket = new VRMetadataPacket{};
			var serializedExpected = JsonUtility.ToJson(vrMetadataPacket);
			var deserializedExpected = JsonUtility.FromJson<VRMetadataPacket>(serializedExpected);

			var serializedActual = PacketUtils.SerializePacket(vrMetadataPacket);
			var deserializedActual = PacketUtils.DeserializePacket(serializedActual.header, serializedActual.contents);
			
			Assert.AreEqual(serializedExpected, serializedActual.contents);
			Assert.AreEqual(deserializedExpected, deserializedActual);
		}
	}
}