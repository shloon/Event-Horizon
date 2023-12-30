using System;
using UnityEngine;

namespace EventHorizon
{
	[DisallowMultipleComponent]
	[DefaultExecutionOrder(-99)]
	[AddComponentMenu("Event Horizon/Trackable")]
	public sealed class Trackable : MonoBehaviour
	{
		[SerializeField] public TrackableID id;
		[NonSerialized] public ITrackableManager manager;

		private void Awake()
		{
			manager = manager ?? TrackableManagerComponent.Instance;
			manager?.Register(this);
		}

		private void OnDestroy() => manager?.Unregister(this);
	}
}