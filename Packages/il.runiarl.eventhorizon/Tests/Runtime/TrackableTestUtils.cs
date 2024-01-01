using System.Collections.Generic;
using UnityEngine;

namespace EventHorizon.Tests.Utilities
{
	public static class TrackableTestUtils
	{
		public static TrackableComponent CreateTrackableGameObject(TrackableID id = new())
		{
			var go = new GameObject("Trackable");
			go.SetActive(false);

			var trackable = go.AddComponent<TrackableComponent>();
			trackable.manager = new DummyManager();
			trackable.Id = id;
			go.SetActive(true);

			return trackable;
		}

		public static void DestroyTrackableGameObject(TrackableComponent trackableComponent)
		{
#if UNITY_EDITOR
			Object.DestroyImmediate(trackableComponent.gameObject);
#else
			Object.Destroy(trackable.gameObject);
#endif
		}

		internal class DummyManager : ITrackableManager
		{
			public IReadOnlyDictionary<TrackableID, ITrackable> RegisteredTrackables => null;
			public void ChangeTrackableID(TrackableID previousID, TrackableID newID) { }

			public TrackableID GenerateId() => TrackableID.Unassigned;
			public void Register(ITrackable trackable) { }
			public void Unregister(ITrackable trackable) { }
		}
	}
}