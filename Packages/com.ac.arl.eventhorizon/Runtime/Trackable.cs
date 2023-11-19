using System;
using UnityEngine;

namespace EventHorizon
{
	[DisallowMultipleComponent]
	public sealed class Trackable : MonoBehaviour
	{
		[SerializeField] public TrackableID id;
		
		private void Awake() => TrackableManager.Instance.Register(this);
		private void OnDestroy() => TrackableManager.Instance.Unregister(this);
	}
}