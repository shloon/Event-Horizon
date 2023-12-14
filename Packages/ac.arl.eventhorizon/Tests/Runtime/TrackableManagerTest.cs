using EventHorizon;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EventHorizon.Tests
{
	public class TrackableManagerTest
	{
		[Test]
		public void Register_WithValidTrackable_ShouldAddTrackable()
		{
			var trackable = CreateTrackable(new TrackableID(1));
			var manager = new TrackableManager();

			Assert.DoesNotThrow(() => manager.Register(trackable));

			DestroyTrackable(trackable);
		}

		[Test]
		public void Register_WithInvalidId_ShouldThrowArgumentException()
		{
			var trackable = CreateTrackable();
			var manager = new TrackableManager();

			Assert.Throws<ArgumentException>(() => manager.Register(trackable));

			DestroyTrackable(trackable);
		}

		[Test]
		public void Register_AlreadyRegisteredTrackable_ShouldThrowInvalidOperationException()
		{
			var trackable = CreateTrackable(new TrackableID(1));
			var manager = new TrackableManager();

			manager.Register(trackable);

			Assert.Throws<InvalidOperationException>(() => manager.Register(trackable));

			DestroyTrackable(trackable);
		}

		[Test]
		public void Register_DifferentTrackableWithSameId_ShouldThrowInvalidOperationException()
		{
			var trackable1 = CreateTrackable(new TrackableID(1));
			var trackable2 = CreateTrackable(new TrackableID(1));
			var manager = new TrackableManager();

			manager.Register(trackable1);

			Assert.Throws<InvalidOperationException>(() => manager.Register(trackable2));

			DestroyTrackable(trackable1);
			DestroyTrackable(trackable2);
		}

		[Test]
		public void Unregister_RegisteredTrackable_ShouldRemoveTrackable()
		{
			var trackable = CreateTrackable(new TrackableID(1));
			var manager = new TrackableManager();
			manager.Register(trackable);

			Assert.DoesNotThrow(() => manager.Unregister(trackable));

			DestroyTrackable(trackable);
		}

		[Test]
		public void Unregister_WithInvalidId_ShouldThrowArgumentException()
		{
			var trackable = CreateTrackable();
			var manager = new TrackableManager();

			Assert.Throws<ArgumentException>(() => manager.Unregister(trackable));

			DestroyTrackable(trackable);
		}

		[Test]
		public void Unregister_UnregisteredTrackable_ShouldThrowInvalidOperationException()
		{
			var trackable = CreateTrackable(new TrackableID(1));
			var manager = new TrackableManager();

			Assert.Throws<InvalidOperationException>(() => manager.Unregister(trackable));

			DestroyTrackable(trackable);
		}

		[Test]
		public void Unregister_DifferentTrackableWithSameRegisteredId_ShouldThrowInvalidOperationException()
		{
			var trackable1 = CreateTrackable(new TrackableID(1));
			var trackable2 = CreateTrackable(new TrackableID(1));
			var manager = new TrackableManager();

			manager.Register(trackable1);

			Assert.Throws<InvalidOperationException>(() => manager.Unregister(trackable2));

			DestroyTrackable(trackable1);
			DestroyTrackable(trackable2);
		}

		[Test]
		public void GenerateId_ShouldReturnUniqueId()
		{
			var trackableManager = new TrackableManager();
			var id = trackableManager.GenerateId();

			Assert.IsTrue(id.IsValid);
			Assert.IsFalse(trackableManager.RegisteredTrackables.ContainsKey(id));
		}

		[Test]
		public void GenerateId_ShouldNotReturnRegisteredId()
		{
			var rng = new PcgRng(1023);
			var trackableManager = new TrackableManager(rng);
			var trackable = CreateTrackable(new TrackableID(10));
			trackableManager.Register(trackable);

			var newId = trackableManager.GenerateId();
			Assert.AreNotEqual(trackable.id, newId);

			DestroyTrackable(trackable);
		}

		[Test]
		public void GenerateId_MaxAttemptsReached_ShouldThrowInvalidOperationException()
		{
			const int constId = 13;
			var mockRng = new Mock<IRandomNumberGenerator>();
			mockRng.Setup(rng => rng.Next()).Returns(constId);

			var manager = new TrackableManager(mockRng.Object);
			var trackable = CreateTrackable(new TrackableID(constId));
			manager.Register(trackable);

			Assert.Throws<InvalidOperationException>(() => manager.GenerateId());

			DestroyTrackable(trackable);
		}

		[Test]
		public void GenerateId_ReturnsValidIdEvenAfterUnregistering()
		{
			const int trackable1Id = 13;
			var mockRng = new Mock<IRandomNumberGenerator>();
			mockRng.Setup(rng => rng.Next()).Returns(trackable1Id);

			var trackableManager = new TrackableManager(mockRng.Object);
			var trackable1 = CreateTrackable(new TrackableID(trackable1Id));
			var trackable2 = CreateTrackable(new TrackableID(trackable1Id + 1));

			trackableManager.Register(trackable1);
			trackableManager.Register(trackable2);
			trackableManager.Unregister(trackable1);

			var newId = trackableManager.GenerateId();

			Assert.IsTrue(newId.IsValid);
			Assert.IsFalse(trackableManager.RegisteredTrackables.ContainsKey(newId));
		}

		[Test]
		public void GenerateId_ContinuousUsageStaysUnique()
		{
			var trackableManager = new TrackableManager();
			var uniqueIds = new HashSet<TrackableID>();

			for (var i = 0; i < TrackableManager.MaxGenerateAttempts * 10; i++)
			{
				var id = trackableManager.GenerateId();
				Assert.IsTrue(id.IsValid);
				Assert.IsTrue(uniqueIds.Add(id), $"Duplicate ID generated: {id}");
			}
		}

		[Test]
		public void GenerateId_UniqueIdsAfterRegisteringMaxMinusOneTrackables()
		{
			var trackableManager = new TrackableManager();

			// Register max-1 trackables
			for (var i = 0; i < TrackableManager.MaxGenerateAttempts - 1; i++)
			{
				var trackable = CreateTrackable(new TrackableID((uint) i + 1));
				trackableManager.Register(trackable);
				DestroyTrackable(trackable);
			}

			var newId = trackableManager.GenerateId();
			Assert.IsTrue(newId.IsValid);
			Assert.IsFalse(trackableManager.RegisteredTrackables.ContainsKey(newId));
		}

		#region Helpers

		private class DummyManager : ITrackableManager
		{
			public IReadOnlyDictionary<TrackableID, Trackable> RegisteredTrackables => null;
			public void Register(Trackable trackable) { }
			public void Unregister(Trackable trackable) { }
			public TrackableID GenerateId() => TrackableID.Unassigned;
		}

		private Trackable CreateTrackable(TrackableID id = new())
		{
			var go = new GameObject("Trackable");
			go.SetActive(false);

			var trackable = go.AddComponent<Trackable>();
			trackable.manager = new DummyManager();
			trackable.id = id;
			go.SetActive(true);

			return trackable;
		}

		private void DestroyTrackable(Trackable trackable) => Object.Destroy(trackable.gameObject);

		#endregion
	}
}