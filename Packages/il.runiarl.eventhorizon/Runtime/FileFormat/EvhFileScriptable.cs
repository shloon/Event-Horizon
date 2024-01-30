using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

namespace EventHorizon.FileFormat
{
	[NoReorder]
	public class EvhFileScriptable : ScriptableObject
	{
		public MetadataPacket metadataPacket;
		public VRMetadataPacket vrMetadataPacket;

		public List<FramePacket> framePackets = new();
		public List<GenericDataPacket> genericDataPackets = new();
		public List<TransformPacket> transformPackets = new();
		public List<ActivationPacket> activationPackets = new();
	}
}