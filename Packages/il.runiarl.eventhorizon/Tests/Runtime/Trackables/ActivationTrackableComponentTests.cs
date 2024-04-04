using EventHorizon.FileFormat;
using EventHorizon.Tests.Utilities;
using EventHorizon.Trackables;
using NUnit.Framework;
using UnityEngine;

namespace EventHorizon.Tests
{
	public class ActivationTrackableComponentTests
	{
		// note that in actuality, activationTrackableComponent is only used when the GO containing it is active
		// we test both states regardless
		[Test]
		public void GetPacketForFrame_AlwaysReturnsPacket()
		{
			var gameObject = new GameObject();
			gameObject.SetActive(false);
			var activationTrackableComponent = gameObject.AddComponent<ActivationTrackableComponent>();
			activationTrackableComponent.Id = new TrackableID(1);
			activationTrackableComponent.manager = new TrackableTestUtils.DummyManager();
			gameObject.SetActive(true);
			
			gameObject.SetActive(true);
			Assert.AreEqual(new ActivationPacket { id = activationTrackableComponent.Id.Internal, frame = 1234 },
				activationTrackableComponent.GetPacketForFrame(1234));
			
			gameObject.SetActive(false);
			Assert.AreEqual(new ActivationPacket { id = activationTrackableComponent.Id.Internal, frame = 4567 },
				activationTrackableComponent.GetPacketForFrame(4567));
			
			Object.Destroy(gameObject);
		}
	}
}