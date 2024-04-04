using EventHorizon.Trackables;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace EventHorizon.Tests.Utilities
{
	public class TestTrackable : ITrackable
	{
		public TestTrackable(TrackableID id = new(), string name = "Trackable")
		{
			Name = name;
			Id = id;
		}

		public TrackableID Id { get; set; }
		public string Name { get; }
	}
	
	public static class UnityAsserts
	{
		private const float DefaultTolerance = 0.00001f;

		public static void AreEqual(Vector3 expected, Vector3 actual, float tolerance = DefaultTolerance) =>
			Assert.AreEqual(0, Vector3.Distance(expected, actual), tolerance,
				$"Expected: {expected}, Actual: {actual}");

		public static void AreEqual(Quaternion expected, Quaternion actual, float tolerance = DefaultTolerance) =>
			Assert.AreEqual(0, Quaternion.Angle(expected, actual), tolerance,
				$"Expected: {expected}, Actual: {actual}");
	}
	
	public static class TrackableTestUtils
	{
		public static TransformTrackableComponent CreateTrackableGameObject(TrackableID id) =>
			CreateTrackableGameObject(new DummyManager(), id);

		public static TransformTrackableComponent CreateTrackableGameObject(ITrackableManager manager) =>
			CreateTrackableGameObject(manager, manager.GenerateId());

		public static TransformTrackableComponent CreateTrackableGameObject(ITrackableManager manager,
			TrackableID id)
		{
			var go = new GameObject("Trackable");
			go.SetActive(false);

			var trackable = go.AddComponent<TransformTrackableComponent>();
			trackable.manager = manager;
			trackable.Id = id;
			go.SetActive(true);

			return trackable;
		}

		public static void DestroyTrackableGameObject(
			TransformTrackableComponent transformPacketGeneratorComponent)
		{
#if UNITY_EDITOR
			Object.DestroyImmediate(transformPacketGeneratorComponent.gameObject);
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