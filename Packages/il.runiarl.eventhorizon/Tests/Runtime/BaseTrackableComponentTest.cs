using EventHorizon.FileFormat;
using EventHorizon.Trackables;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace EventHorizon.Tests
{
	struct DummyPacket : IPacket
	{
		public PacketType Type => PacketType.Undefined;
	}

	internal class DummyTrackableComponent : BaseTrackableComponent<DummyPacket>
	{
		public override DummyPacket GetPacketForFrame(ulong frame) => new DummyPacket();
	}
	
	public class BaseTrackableComponentTest
	{
		private GameObject gameObject;
		private Mock<ITrackableManager> mockManager;

		[SetUp]
		public void SetUp()
		{
			mockManager = new Mock<ITrackableManager>();
			gameObject = new GameObject("trackable");
		}

		[TearDown]
		public void TearDown()
		{
			if (gameObject == null)
			{
				Object.Destroy(gameObject);
			}
		}

		[Test]
		public void OnEnable_WithManagerSet_ShouldCallRegister()
		{
			gameObject.SetActive(false);
			var trackableComponent = gameObject.AddComponent<DummyTrackableComponent>();
			trackableComponent.Id = new TrackableID(1234);
			trackableComponent.manager = mockManager.Object;

			gameObject.SetActive(true);

			mockManager.Verify(m => m.Register(trackableComponent), Times.Once());
		}

		[Test]
		public void OnEnable_WithoutManagerSet_ShouldFallbackToSingletonAndCallRegister()
		{
			gameObject.SetActive(false);
			var trackableComponent = gameObject.AddComponent<DummyTrackableComponent>();
			trackableComponent.Id = new TrackableID(1234);
			var singletonManager = new GameObject().AddComponent<TrackableManagerComponent>();

			gameObject.SetActive(true);

			Assert.AreEqual(singletonManager, trackableComponent.manager);
			Assert.AreEqual(TrackableManagerComponent.Instance, trackableComponent.manager);

			Object.Destroy(singletonManager.gameObject);
		}


		[Test]
		public void OnDisable_ShouldCallUnregister()
		{
			gameObject.SetActive(false);
			var trackableComponent = gameObject.AddComponent<DummyTrackableComponent>();
			trackableComponent.Id = new TrackableID(1234);
			trackableComponent.manager = mockManager.Object;
			gameObject.SetActive(true);

			trackableComponent.enabled = false;
			mockManager.Verify(m => m.Unregister(trackableComponent), Times.Once());
		}

		[Test]
		public void OnDestroy_ShouldCallUnregister()
		{
			gameObject.SetActive(false);
			var trackableComponent = gameObject.AddComponent<DummyTrackableComponent>();
			trackableComponent.Id = new TrackableID(1234);
			trackableComponent.manager = mockManager.Object;
			gameObject.SetActive(true);

			Object.Destroy(trackableComponent);

			mockManager.Verify(m => m.Unregister(trackableComponent), Times.Once());
		}

		[Test]
		public void Name_ReturnsGameObjectName()
		{
			gameObject.SetActive(false);
			var trackableComponent = gameObject.AddComponent<DummyTrackableComponent>();
			trackableComponent.Id = new TrackableID(1234);
			trackableComponent.manager = mockManager.Object;
			gameObject.SetActive(true);

			Assert.AreEqual(gameObject.name, trackableComponent.Name);
		}
	}
}