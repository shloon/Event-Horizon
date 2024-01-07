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
		public bool isLocal;
		private Transform selfTransform;

		public void Start() => selfTransform = transform;

		public override TransformPacket GetPacketForFrame(ulong frame)
		{
			Vector3 translation;
			Quaternion rotation;
			if (isLocal)
			{
				transform.GetLocalPositionAndRotation(out translation, out rotation);
			}
			else
			{
				transform.GetPositionAndRotation(out translation, out rotation);
			}

			return new TransformPacket
			{
				frame = frame,
				id = Id.Internal,
				isLocal = isLocal,
				translation = translation,
				rotation = rotation,
				scale = selfTransform.localScale
			};
		}
	}
}