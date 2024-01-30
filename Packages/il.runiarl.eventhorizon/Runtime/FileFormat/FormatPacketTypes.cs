using System;
using System.Globalization;
using UnityEngine;

namespace EventHorizon.FileFormat
{
	public enum PacketType
	{
		Undefined,
		Metadata,
		Frame,
		Transform,
		Generic,
		VRMetadata,
		Activation
	}

	public interface IPacket
	{
		public PacketType Type { get; }
	}

	[Serializable]
	public struct PacketHeader
	{
		public PacketType type;
	}

	[Serializable]
	public struct GenericDataPacket : IPacket
	{
		public ulong frame;
		public string data;
		public PacketType Type => PacketType.Generic;
	}

	[Serializable]
	public struct FramePacket : IPacket
	{
		public ulong frame;
		public double elapsedTime;
		public PacketType Type => PacketType.Frame;
	}

	[Serializable]
	public struct MetadataPacket : IPacket
	{
		public RecordingFormatVersion version;
		public string sceneName;
		public FrameRate fps;
		public string timestamp;

		public PacketType Type => PacketType.Metadata;
	}

	[Serializable]
	public struct VRMetadataPacket : IPacket
	{
		public string headsetType;
		public string interactionProfile;
		public PacketType Type => PacketType.VRMetadata;
	}

	[Serializable]
	public struct ActivationPacket : IPacket
	{
		public ulong frame;
		public uint id;
		public PacketType Type => PacketType.Activation;
	}

	[Serializable]
	public struct TransformPacket : IPacket
	{
		public ulong frame;
		public uint id;
		public bool isLocal;
		public Vector3 translation;
		public Quaternion rotation;
		public Vector3 scale;
		public PacketType Type => PacketType.Transform;
	}

	[Serializable]
	public struct SerializedPacketData
	{
		public string header;
		public string contents;
	}

	public static class PacketUtils
	{
		public static PacketHeader GenerateHeader<T>(in T packet) where T : IPacket => new() { type = packet.Type };

		public static SerializedPacketData SerializePacket<T>(in T packet) where T : IPacket =>
			new() { header = JsonUtility.ToJson(GenerateHeader(packet)), contents = JsonUtility.ToJson(packet) };

		public static IPacket DeserializePacket(in string header, in string contents)
		{
			var packetHeader = JsonUtility.FromJson<PacketHeader>(header);

			return packetHeader.type switch
			{
				PacketType.Metadata => JsonUtility.FromJson<MetadataPacket>(contents),
				PacketType.Transform => JsonUtility.FromJson<TransformPacket>(contents),
				PacketType.Generic => JsonUtility.FromJson<GenericDataPacket>(contents),
				PacketType.Frame => JsonUtility.FromJson<FramePacket>(contents),
				PacketType.Activation => JsonUtility.FromJson<ActivationPacket>(contents),
				PacketType.VRMetadata => JsonUtility.FromJson<VRMetadataPacket>(contents),
				PacketType.Undefined => null,
				_ => null
			};
		}

		public static MetadataPacket GenerateMetadataPacket(string sceneName, FrameRate fps, DateTime time) =>
			new()
			{
				version = RecordingFormatVersion.V1,
				sceneName = sceneName,
				fps = fps,
				timestamp = time.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture)
			};
	}
}