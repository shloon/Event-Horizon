using System.Collections.Generic;
using UnityEngine;

namespace EventHorizon.Tests.Utilities
{
	public static class TrackableTestUtils
	{
		public static Trackable CreateTrackable(TrackableID id = new())
		{
			var go = new GameObject("Trackable");
			go.SetActive(false);

			var trackable = go.AddComponent<Trackable>();
			trackable.manager = new DummyManager();
			trackable.id = id;
			go.SetActive(true);

			return trackable;
		}

		public static void DestroyTrackable(Trackable trackable)
		{
#if UNITY_EDITOR
			Object.DestroyImmediate(trackable.gameObject);
#else
			Object.Destroy(trackable.gameObject);
#endif
		}

		internal class DummyManager : ITrackableManager
		{
			public IReadOnlyDictionary<TrackableID, Trackable> RegisteredTrackables => null;
			public void Register(Trackable trackable) { }
			public void Unregister(Trackable trackable) { }
			public void ChangeTrackableID(TrackableID previousID, TrackableID newID) { }

			public TrackableID GenerateId() => TrackableID.Unassigned;
		}
	}
}