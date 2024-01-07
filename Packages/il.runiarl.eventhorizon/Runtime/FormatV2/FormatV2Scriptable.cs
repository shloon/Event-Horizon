using EventHorizon.FormatV2;
using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

namespace EventHorizon.Editor.RecordingsV2
{
	[NoReorder]
	public class FormatV2Scriptable : ScriptableObject
	{
		public MetadataPacket metadataPacket;

		public List<FramePacket> framePackets = new();
		public List<GenericDataPacket> genericDataPackets = new();
		public List<TransformPacket> transformPackets = new();
		public List<ActivationPacket> activationPackets = new();
	}
}