using EventHorizon.FormatV2;
using UnityEngine;

namespace EventHorizon
{
	[DisallowMultipleComponent]
	[DefaultExecutionOrder(-99)]
	[AddComponentMenu("Event Horizon/Activation Trackable")]
	[ExecuteAlways]
	public sealed class ActivationTrackableComponent : BaseTrackableComponent<ActivationPacket>
	{
		// no needs for special checks against gameObject.enabled or the likes of it here, because:
		//	- This function is queried only when the component is active
		//	- Which is in turn where the object is active
		//  - Which is what we desire
		// when it is not active, it doesn't emit any activation packets, and that's exactly what we want!
		public override ActivationPacket GetPacketForFrame(ulong frame) => new() { frame = frame, id = Id.Internal };
	}
}