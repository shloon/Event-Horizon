using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace EventHorizon
{
	[DisallowMultipleComponent]
	[DefaultExecutionOrder(-99)]
	[AddComponentMenu("Event Horizon/Trackable")]
	[ExecuteAlways]
	public sealed class TrackableComponent : MonoBehaviour, ITrackable
	{
		public ITrackableManager manager;

		private void Awake()
		{
			manager = manager ?? TrackableManagerComponent.Instance;
#if UNITY_EDITOR
			// Generate key if in Edit Mode
			if (manager != null && !EditorApplication.isPlayingOrWillChangePlaymode)
			{
				if (!Id.IsValid)
				{
					Id = manager.GenerateId();
				}
			}
#endif
			manager?.Register(this);
		}

		private void OnDestroy() => manager?.Unregister(this);

		[field: SerializeField]
		[field: FormerlySerializedAs("id")]
		public TrackableID Id { get; set; }

		public string Name => gameObject.name;
	}
}