using EventHorizon.Tests.Utilities;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace EventHorizon.Tests
{
	public class TrackableManagerTest
	{
		[Test]
		public void Register_WithValidTrackable_ShouldAddTrackable()
		{
			var trackable = TrackableTestUtils.CreateTrackableGameObject(new TrackableID(1));
			var manager = new TrackableManager();

			Assert.DoesNotThrow(() => manager.Register(trackable));

			TrackableTestUtils.DestroyTrackableGameObject(trackable);
		}

		[Test]
		public void Register_WithInvalidId_ShouldThrowArgumentException()
		{
			var trackable = TrackableTestUtils.CreateTrackableGameObject();
			var manager = new TrackableManager();

			Assert.Throws<ArgumentException>(() => manager.Register(trackable));

			TrackableTestUtils.DestroyTrackableGameObject(trackable);
		}

		[Test]
		public void Register_AlreadyRegisteredTrackable_ShouldThrowInvalidOperationException()
		{
			var trackable = TrackableTestUtils.CreateTrackableGameObject(new TrackableID(1));
			var manager = new TrackableManager();

			manager.Register(trackable);

			Assert.Throws<InvalidOperationException>(() => manager.Register(trackable));

			TrackableTestUtils.DestroyTrackableGameObject(trackable);
		}

		[Test]
		public void Register_DifferentTrackableWithSameId_ShouldThrowInvalidOperationException()
		{
			var trackable1 = TrackableTestUtils.CreateTrackableGameObject(new TrackableID(1));
			var trackable2 = TrackableTestUtils.CreateTrackableGameObject(new TrackableID(1));
			var manager = new TrackableManager();

			manager.Register(trackable1);

			Assert.Throws<InvalidOperationException>(() => manager.Register(trackable2));

			TrackableTestUtils.DestroyTrackableGameObject(trackable1);
			TrackableTestUtils.DestroyTrackableGameObject(trackable2);
		}

		[Test]
		public void ChangeTrackableID_DifferentTrackableDifferentID_ShouldSucceed()
		{
			var expectedTrackableID = new TrackableID(3);
			var trackable1 = TrackableTestUtils.CreateTrackableGameObject(new TrackableID(1));
			var manager = new TrackableManager();
			manager.Register(trackable1);

			manager.ChangeTrackableID(trackable1.Id, expectedTrackableID);

			Assert.IsTrue(manager.RegisteredTrackables.ContainsKey(expectedTrackableID));
			Assert.IsFalse(manager.RegisteredTrackables.ContainsKey(trackable1.Id));

			TrackableTestUtils.DestroyTrackableGameObject(trackable1);
		}

		[Test]
		public void ChangeTrackableID_NonExistentTrackableReferenced_ShouldThrowArgumentException()
		{
			var trackable1 = TrackableTestUtils.CreateTrackableGameObject(new TrackableID(1));
			var manager = new TrackableManager();

			manager.Register(trackable1);

			Assert.Throws<ArgumentException>(() => manager.ChangeTrackableID(new TrackableID(303), trackable1.Id));

			TrackableTestUtils.DestroyTrackableGameObject(trackable1);
		}

		[Test]
		public void ChangeTrackableID_DifferentTrackableWithSameId_ShouldThrowInvalidOperationException()
		{
			var trackable1 = TrackableTestUtils.CreateTrackableGameObject(new TrackableID(1));
			var trackable2 = TrackableTestUtils.CreateTrackableGameObject(new TrackableID(2));
			var manager = new TrackableManager();

			manager.Register(trackable1);
			manager.Register(trackable2);

			Assert.Throws<InvalidOperationException>(() => manager.ChangeTrackableID(trackable2.Id, trackable1.Id));

			TrackableTestUtils.DestroyTrackableGameObject(trackable1);
			TrackableTestUtils.DestroyTrackableGameObject(trackable2);
		}

		[Test]
		public void ChangeTrackableID_SameTrackable_NoOp()
		{
			var trackable1 = TrackableTestUtils.CreateTrackableGameObject(new TrackableID(1));
			var manager = new TrackableManager();
			manager.Register(trackable1);

			manager.ChangeTrackableID(trackable1.Id, trackable1.Id);

			Assert.AreEqual(new TrackableID(1), trackable1.Id);

			TrackableTestUtils.DestroyTrackableGameObject(trackable1);
		}

		[Test]
		public void Unregister_RegisteredTrackable_ShouldRemoveTrackable()
		{
			var trackable = TrackableTestUtils.CreateTrackableGameObject(new TrackableID(1));
			var manager = new TrackableManager();
			manager.Register(trackable);

			Assert.DoesNotThrow(() => manager.Unregister(trackable));

			TrackableTestUtils.DestroyTrackableGameObject(trackable);
		}

		[Test]
		public void Unregister_WithInvalidId_ShouldThrowArgumentException()
		{
			var trackable = TrackableTestUtils.CreateTrackableGameObject();
			var manager = new TrackableManager();

			Assert.Throws<ArgumentException>(() => manager.Unregister(trackable));

			TrackableTestUtils.DestroyTrackableGameObject(trackable);
		}

		[Test]
		public void Unregister_UnregisteredTrackable_ShouldThrowInvalidOperationException()
		{
			var trackable = TrackableTestUtils.CreateTrackableGameObject(new TrackableID(1));
			var manager = new TrackableManager();

			Assert.Throws<InvalidOperationException>(() => manager.Unregister(trackable));

			TrackableTestUtils.DestroyTrackableGameObject(trackable);
		}

		[Test]
		public void Unregister_DifferentTrackableWithSameRegisteredId_ShouldThrowInvalidOperationException()
		{
			var trackable1 = TrackableTestUtils.CreateTrackableGameObject(new TrackableID(1));
			var trackable2 = TrackableTestUtils.CreateTrackableGameObject(new TrackableID(1));
			var manager = new TrackableManager();

			manager.Register(trackable1);

			Assert.Throws<InvalidOperationException>(() => manager.Unregister(trackable2));

			TrackableTestUtils.DestroyTrackableGameObject(trackable1);
			TrackableTestUtils.DestroyTrackableGameObject(trackable2);
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
			var trackable = TrackableTestUtils.CreateTrackableGameObject(new TrackableID(10));
			trackableManager.Register(trackable);

			var newId = trackableManager.GenerateId();
			Assert.AreNotEqual(trackable.Id, newId);

			TrackableTestUtils.DestroyTrackableGameObject(trackable);
		}

		[Test]
		public void GenerateId_MaxAttemptsReached_ShouldThrowInvalidOperationException()
		{
			const int constId = 13;
			var mockRng = new Mock<IRandomNumberGenerator>();
			mockRng.Setup(rng => rng.Next()).Returns(constId);

			var manager = new TrackableManager(mockRng.Object);
			var trackable = TrackableTestUtils.CreateTrackableGameObject(new TrackableID(constId));
			manager.Register(trackable);

			Assert.Throws<InvalidOperationException>(() => manager.GenerateId());

			TrackableTestUtils.DestroyTrackableGameObject(trackable);
		}

		[Test]
		public void GenerateId_ReturnsValidIdEvenAfterUnregistering()
		{
			const int trackable1Id = 13;
			var mockRng = new Mock<IRandomNumberGenerator>();
			mockRng.Setup(rng => rng.Next()).Returns(trackable1Id);

			var trackableManager = new TrackableManager(mockRng.Object);
			var trackable1 = TrackableTestUtils.CreateTrackableGameObject(new TrackableID(trackable1Id));
			var trackable2 = TrackableTestUtils.CreateTrackableGameObject(new TrackableID(trackable1Id + 1));

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
				var trackable = TrackableTestUtils.CreateTrackableGameObject(new TrackableID((uint) i + 1));
				trackableManager.Register(trackable);
				TrackableTestUtils.DestroyTrackableGameObject(trackable);
			}

			var newId = trackableManager.GenerateId();
			Assert.IsTrue(newId.IsValid);
			Assert.IsFalse(trackableManager.RegisteredTrackables.ContainsKey(newId));
		}
	}
}