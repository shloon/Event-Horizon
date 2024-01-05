using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace EventHorizon
{
	[DisallowMultipleComponent]
	[DefaultExecutionOrder(-99)]
	[AddComponentMenu("Event Horizon/Trackable")]
	[ExecuteAlways]
	public class BaseTrackableComponent : MonoBehaviour, ITrackable
	{
		private bool isInitialized;
		public ITrackableManager manager;

		private void Awake()
		{
			if (!EditorApplication.isPlayingOrWillChangePlaymode)
			{
				Initialize();
			}
		}

		private void Start() => Initialize();
		private void OnDisable() => manager?.Unregister(this);

		[field: SerializeField]
		[field: FormerlySerializedAs("id")]
		public TrackableID Id { get; set; }

		public string Name => gameObject.name;

		private void Initialize()
		{
			if (isInitialized)
			{
				return;
			}

			manager ??= TrackableManagerComponent.Instance;
#if UNITY_EDITOR
			// Generate key if in Edit Mode and no key was assigned
			if (manager != null && !EditorApplication.isPlayingOrWillChangePlaymode && !Id.IsValid)
			{
				Id = manager.GenerateId();
			}
#endif
			manager?.Register(this);
			isInitialized = true;
		}
	}
}