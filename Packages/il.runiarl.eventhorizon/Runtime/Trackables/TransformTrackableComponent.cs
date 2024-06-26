using EventHorizon.FileFormat;
using UnityEngine;

namespace EventHorizon.Trackables
{
	[DisallowMultipleComponent]
	[DefaultExecutionOrder(-99)]
	[AddComponentMenu("Event Horizon/Transform Trackable")]
	[ExecuteAlways]
	public sealed class TransformTrackableComponent : BaseTrackableComponent<TransformPacket>
	{
		public bool isLocal;
		public Vector3 translationMultiply = new(1, 1, 1);
		public Vector3 rotationMultiply = new(1, 1, 1);
		private Transform selfTransform;

		private void Awake() => selfTransform = transform;

		public override TransformPacket GetPacketForFrame(ulong frame)
		{
			Vector3 translation;
			Quaternion rotation;

			if (isLocal)
			{
				selfTransform.GetLocalPositionAndRotation(out translation, out rotation);
			}
			else
			{
				selfTransform.GetPositionAndRotation(out translation, out rotation);
			}

			var finalTranslation = new Vector3(translation.x * translationMultiply.x,
				translation.y * translationMultiply.y,
				translation.z * translationMultiply.z);

			var rotationEulerAngles = rotation.eulerAngles;
			var finalRotation = Quaternion.Euler(rotationEulerAngles.x * rotationMultiply.x,
				rotationEulerAngles.y * rotationMultiply.y,
				rotationEulerAngles.z * rotationMultiply.z);

			return new TransformPacket
			{
				frame = frame,
				id = Id.Internal,
				isLocal = isLocal,
				translation =
					finalTranslation,
				rotation = finalRotation,
				scale = selfTransform.localScale
			};
		}
	}
}