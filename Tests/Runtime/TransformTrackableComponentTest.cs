using Moq;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace EventHorizon.Tests
{
	public class TransformPacketGeneratorComponentTest
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

		[UnityTest]
		public IEnumerator Awake_WithManagerSet_ShouldCallRegister()
		{
			var trackableComponent = gameObject.AddComponent<TransformTrackableComponent>();
			trackableComponent.Id = new TrackableID(1234);
			trackableComponent.manager = mockManager.Object;
			yield return null;

			mockManager.Verify(m => m.Register(trackableComponent), Times.Once());
		}

		[UnityTest]
		public IEnumerator Awake_WithoutManagerSet_ShouldFallbackToSingletonAndCallRegister()
		{
			var trackableComponent = gameObject.AddComponent<TransformTrackableComponent>();
			trackableComponent.Id = new TrackableID(1234);

			var singletonManager = new GameObject().AddComponent<TrackableManagerComponent>();
			yield return null;

			Assert.AreEqual(singletonManager, trackableComponent.manager);
			Assert.AreEqual(TrackableManagerComponent.Instance, trackableComponent.manager);

			Object.Destroy(singletonManager.gameObject);
			yield return null;
		}

		[UnityTest]
		public IEnumerator OnDestroy_ShouldCallUnregister()
		{
			var trackableComponent = gameObject.AddComponent<TransformTrackableComponent>();
			trackableComponent.Id = new TrackableID(1234);
			trackableComponent.manager = mockManager.Object;
			yield return null;

			Object.Destroy(trackableComponent);
			yield return null;

			mockManager.Verify(m => m.Unregister(trackableComponent), Times.Once());
		}
	}
}