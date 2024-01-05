using EventHorizon.FormatV2;
using UnityEngine;

namespace EventHorizon
{
	[DisallowMultipleComponent]
	[DefaultExecutionOrder(-99)]
	[AddComponentMenu("Event Horizon/Transform Trackable")]
	[ExecuteAlways]
	public sealed class TransformTrackableComponent : BaseTrackableComponent<TransformPacket>
	{
		public override TransformPacket GetPacketForFrame(ulong frame)
		{
			Transform selfTransform;
			return new TransformPacket
			{
				frame = frame,
				id = Id.Internal,
				translation = (selfTransform = transform).position,
				rotation = selfTransform.rotation,
				scale = selfTransform.localScale
			};
		}
	}
}