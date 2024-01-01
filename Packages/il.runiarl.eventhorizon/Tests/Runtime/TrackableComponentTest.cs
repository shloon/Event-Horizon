using Moq;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace EventHorizon.Tests
{
	public class TrackableComponentTest
	{
		private GameObject gameObject;
		private Mock<ITrackableManager> mockManager;
		private TrackableComponent trackableComponent;

		[SetUp]
		public void SetUp()
		{
			mockManager = new Mock<ITrackableManager>();

			gameObject = new GameObject("trackable");
			gameObject.SetActive(false);

			trackableComponent = gameObject.AddComponent<TrackableComponent>();
			trackableComponent.Id = new TrackableID(1234);
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
		public void Awake_WithManagerSet_ShouldCallRegister()
		{
			trackableComponent.manager = mockManager.Object;

			gameObject.SetActive(true); // This triggers the Awake method

			mockManager.Verify(m => m.Register(trackableComponent), Times.Once());
		}

		[Test]
		public void Awake_WithoutManagerSet_ShouldFallbackToSingletonAndCallRegister()
		{
			var singletonManager = new GameObject().AddComponent<TrackableManagerComponent>();

			gameObject.SetActive(true); // This triggers the Awake method

			Assert.AreEqual(singletonManager, trackableComponent.manager);
			Assert.AreEqual(TrackableManagerComponent.Instance, trackableComponent.manager);

			// cleanup
			Object.Destroy(singletonManager.gameObject);
		}

		[UnityTest]
		public IEnumerator OnDestroy_ShouldCallUnregister()
		{
			trackableComponent.manager = mockManager.Object;
			gameObject.SetActive(true); // This triggers the Awake method

			Object.Destroy(trackableComponent);
			yield return null;

			mockManager.Verify(m => m.Unregister(trackableComponent), Times.Once());
		}
	}
}