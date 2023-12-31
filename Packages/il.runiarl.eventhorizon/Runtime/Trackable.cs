using System;
using UnityEditor;
using UnityEngine;

namespace EventHorizon
{
	[DisallowMultipleComponent]
	[DefaultExecutionOrder(-99)]
	[AddComponentMenu("Event Horizon/Trackable")]
	[ExecuteAlways]
	public sealed class Trackable : MonoBehaviour
	{
		[SerializeField] public TrackableID id;
		[NonSerialized] public ITrackableManager manager;

		private void Awake()
		{
			manager = manager ?? TrackableManagerComponent.Instance;
#if UNITY_EDITOR
			// Generate key if in Edit Mode
			if (manager != null && !EditorApplication.isPlayingOrWillChangePlaymode)
			{
				if (!id.IsValid)
				{
					id = manager.GenerateId();
				}
			}
#endif
			manager?.Register(this);
		}

		private void OnDestroy() => manager?.Unregister(this);
	}
}