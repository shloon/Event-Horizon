using EventHorizon.Tests.Utilities;
using EventHorizon.Trackables;
using NUnit.Framework;
using UnityEngine;

namespace EventHorizon.Tests
{
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

	public class TransformTrackableComponentTests
	{
		private GameObject rootGO;
		private GameObject trackableGO;
		private TransformTrackableComponent transformTrackable;

		[SetUp]
		public void SetUp()
		{
			rootGO = new GameObject();
			transformTrackable = TrackableTestUtils.CreateTrackableGameObject(new TrackableID(1));
			trackableGO = transformTrackable.gameObject;
			trackableGO.transform.SetParent(rootGO.transform);
		}

		[TearDown]
		public void TearDown()
		{
			TrackableTestUtils.DestroyTrackableGameObject(transformTrackable);
			Object.Destroy(rootGO);
		}

		[Test]
		public void Awake_DefaultTranslationMultiply_IsOne() =>
			UnityAsserts.AreEqual(Vector3.one, transformTrackable.translationMultiply);

		[Test]
		public void Awake_DefaultRotationMultiply_IsOne() =>
			UnityAsserts.AreEqual(Vector3.one, transformTrackable.rotationMultiply);

		[Test]
		public void GetPacketForFrame_WithLocalTrue_ReturnsCorrectLocalTransformations()
		{
			var rotation = Quaternion.Euler(4, 5, 6);
			var position = new Vector3(1, 2, 3);
			rootGO.transform.SetLocalPositionAndRotation(position, Quaternion.Inverse(rotation));
			trackableGO.transform.SetLocalPositionAndRotation(position, rotation);

			transformTrackable.isLocal = true;

			var packet = transformTrackable.GetPacketForFrame(0);

			UnityAsserts.AreEqual(position, packet.translation);
			UnityAsserts.AreEqual(rotation, packet.rotation);
		}

		[Test]
		public void GetPacketForFrame_WithLocalFalse_ReturnsCorrectGlobalTransformations()
		{
			var position = new Vector3(1, 2, 3);
			var rotation1 = Quaternion.Euler(4, 5, 6);
			var rotation2 = Quaternion.Euler(7,8,9);
			rootGO.transform.SetLocalPositionAndRotation(position, rotation1);
			trackableGO.transform.SetPositionAndRotation(position, rotation2);

			transformTrackable.isLocal = false;

			var packet = transformTrackable.GetPacketForFrame(0);

			UnityAsserts.AreEqual(position, packet.translation);
			UnityAsserts.AreEqual(rotation2, packet.rotation);
		}

		[Test]
		public void GetPacketForFrame_ApplyTranslationMultiplier_ReturnsCorrectTranslation()
		{
			var translationMultiply = new Vector3(1, 2, 3);
			var position = new Vector3(3, 2, 1);
			var expectedTranslation = new Vector3(3, 4, 3);

			trackableGO.transform.SetPositionAndRotation(position, Quaternion.identity);
			transformTrackable.translationMultiply = translationMultiply;

			var packet = transformTrackable.GetPacketForFrame(0);

			UnityAsserts.AreEqual(expectedTranslation, packet.translation);
		}

		[Test]
		public void GetPacketForFrame_ApplyRotationMultiplier_ReturnsCorrectRotation()
		{
			var rotationMultiply = new Vector3(1, 2, 3);
			var rotation = Quaternion.Euler(3, 2, 1);
			var expectedRotation = Quaternion.Euler(3, 4, 3);

			trackableGO.transform.SetPositionAndRotation(Vector3.zero, rotation);
			transformTrackable.rotationMultiply = rotationMultiply;

			var packet = transformTrackable.GetPacketForFrame(0);

			UnityAsserts.AreEqual(expectedRotation, packet.rotation);
		}

		[Test]
		public void GetPacketForFrame_WithSpecificFrame_ReturnsMatchingFrameNumber()
		{
			var expectedFrameNumber = 1234u;

			var packet = transformTrackable.GetPacketForFrame(expectedFrameNumber);

			Assert.AreEqual(expectedFrameNumber, packet.frame);
		}

		[Test]
		public void GetPacketForFrame_ScaleReflectsSelfTransformLocalScale()
		{
			var scale = new Vector3(1, 2, 3);
			trackableGO.transform.localScale = scale;

			var packet = transformTrackable.GetPacketForFrame(0);

			UnityAsserts.AreEqual(scale, packet.scale);
		}

		[Test]
		public void GetPacketForFrame_WithZeroMultipliers_ReturnsZeroTransformations()
		{
			var position = new Vector3(3, 2, 1);
			var rotation = Quaternion.Euler(3, 2, 1);

			trackableGO.transform.SetPositionAndRotation(position, rotation);
			transformTrackable.translationMultiply = Vector3.zero;
			transformTrackable.rotationMultiply = Vector3.zero;

			var packet = transformTrackable.GetPacketForFrame(0);

			UnityAsserts.AreEqual(Vector3.zero, packet.translation);
			UnityAsserts.AreEqual(Quaternion.identity, packet.rotation);
		}

		[Test]
		public void GetPacketForFrame_WithNegativeMultipliers_HandlesNegativesCorrectly()
		{
			var position = new Vector3(3, 2, 1);
			var rotation = Quaternion.Euler(3, 2, 1);

			trackableGO.transform.SetPositionAndRotation(position, rotation);
			transformTrackable.translationMultiply = -Vector3.one;
			transformTrackable.rotationMultiply = -Vector3.one;

			var packet = transformTrackable.GetPacketForFrame(0);

			UnityAsserts.AreEqual(-position, packet.translation);
			UnityAsserts.AreEqual(Quaternion.Euler(-3, -2, -1), packet.rotation);
		}
	}
}