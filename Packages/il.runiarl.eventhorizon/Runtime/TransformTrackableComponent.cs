using EventHorizon.FormatV2;
using UnityEngine;

namespace EventHorizon
{
	[DisallowMultipleComponent]
	[DefaultExecutionOrder(-99)]
	[AddComponentMenu("Event Horizon/Transform Trackable")]
	[ExecuteAlways]
	public sealed class TransformTrackableComponent : BaseTrackableComponent
	{
	}
}