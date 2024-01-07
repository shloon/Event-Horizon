using EventHorizon.FormatV2;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace EventHorizon
{
	public abstract class BaseTrackableComponent<T> : MonoBehaviour, ITrackable, IPacketGenerator<T> where T : IPacket
	{
		private bool isInitialized;
		public ITrackableManager manager;

		private void OnEnable()
		{
			Initialize();
			if (Id.IsValid)
			{
				manager?.Register(this);
			}
		}

		private void OnDisable() => manager?.Unregister(this);

		public abstract T GetPacketForFrame(ulong frame);

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
			isInitialized = true;
		}
	}
}