using System;
using System.Diagnostics.CodeAnalysis;
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
	[ExcludeFromCodeCoverage]
	public struct PacketHeader
	{
		public PacketType type;
	}

	[Serializable]
	[ExcludeFromCodeCoverage]
	public struct GenericDataPacket : IPacket
	{
		public ulong frame;
		public string data;
		public PacketType Type => PacketType.Generic;
	}

	[Serializable]
	[ExcludeFromCodeCoverage]
	public struct FramePacket : IPacket
	{
		public ulong frame;
		public double elapsedTime;
		public PacketType Type => PacketType.Frame;
	}

	[Serializable]
	[ExcludeFromCodeCoverage]
	public struct MetadataPacket : IPacket
	{
		public RecordingFormatVersion version;
		public string sceneName;
		public FrameRate fps;
		public string timestamp;

		public PacketType Type => PacketType.Metadata;
	}

	[Serializable]
	[ExcludeFromCodeCoverage]
	public struct VRMetadataPacket : IPacket
	{
		public string headsetType;
		public string interactionProfile;
		public PacketType Type => PacketType.VRMetadata;
	}

	[Serializable]
	[ExcludeFromCodeCoverage]
	public struct ActivationPacket : IPacket
	{
		public ulong frame;
		public uint id;
		public PacketType Type => PacketType.Activation;
	}

	[Serializable]
	[ExcludeFromCodeCoverage]
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
	[ExcludeFromCodeCoverage]
	public struct SerializedPacketData
	{
		public string header;
		public string contents;
	}
}