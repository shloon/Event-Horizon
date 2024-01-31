using System;
using System.Globalization;
using UnityEngine;

namespace EventHorizon.FileFormat
{
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