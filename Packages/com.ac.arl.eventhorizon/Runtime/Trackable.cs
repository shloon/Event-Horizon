using System;
using UnityEngine;

namespace EventHorizon
{
	[DisallowMultipleComponent]
	public sealed class Trackable : MonoBehaviour
	{
		[SerializeField] public TrackableID id;
		[HideInInspector] public ITrackableManager manager;

		private void Awake() { manager = manager ?? TrackableManagerComponent.Instance; manager.Register(this); }
		private void OnDestroy() => manager.Unregister(this);
	}
}