using System;
using UnityEngine;

namespace EventHorizon
{
	[DisallowMultipleComponent]
	public sealed class Trackable : MonoBehaviour
	{
		[SerializeField] public TrackableID id;
	
		private void Awake() => TrackableManagerComponent.Instance.Register(this);
		private void OnDestroy() => TrackableManagerComponent.Instance.Unregister(this);
	}
}