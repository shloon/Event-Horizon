using Moq;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace EventHorizon.Tests
{
	public class TrackableTest
	{
		private Mock<ITrackableManager> mockManager;
		private GameObject gameObject;
		private Trackable trackable;

		[SetUp]
		public void SetUp()
		{
			mockManager = new Moq.Mock<ITrackableManager>();

			gameObject = new GameObject("trackable");
			gameObject.SetActive(false);

			trackable = gameObject.AddComponent<Trackable>();
			trackable.id = new TrackableID(1234);
		}

		[TearDown]
		public void TearDown()
		{
			if (gameObject == null)
				Object.Destroy(gameObject);
		}

		[Test]
		public void Awake_WithManagerSet_ShouldCallRegister()
		{
			trackable.manager = mockManager.Object;

			gameObject.SetActive(true); // This triggers the Awake method

			mockManager.Verify(m => m.Register(trackable), Times.Once());
		}

		[Test]
		public void Awake_WithoutManagerSet_ShouldFallbackToSingletonAndCallRegister()
		{
			var singletonManager = new GameObject().AddComponent<TrackableManagerComponent>();

			gameObject.SetActive(true); // This triggers the Awake method

			Assert.AreEqual(singletonManager, trackable.manager);
			Assert.AreEqual(TrackableManagerComponent.Instance, trackable.manager);

			// cleanup
			Object.Destroy(singletonManager.gameObject);
		}

		[UnityTest]
		public IEnumerator OnDestroy_ShouldCallUnregister()
		{
			trackable.manager = mockManager.Object;
			gameObject.SetActive(true); // This triggers the Awake method

			Object.Destroy(trackable);
			yield return null;

			mockManager.Verify(m => m.Unregister(trackable), Times.Once());
		}
	}
}