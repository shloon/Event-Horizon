using NUnit.Framework;
using UnityEngine;

namespace EventHorizon.Tests
{
	[Parallelizable]
	public class TrackableIDTests
	{
		[Test]
		public void Constructor_AssignsCorrectValue()
		{
			uint id = 123;
			TrackableID trackableID = new TrackableID(id);

			Assert.AreEqual(id, trackableID.Internal);
		}

		[Test]
		public void Constructor_DefaultId_IsNotValid()
		{
			TrackableID defaultId = TrackableID.Unassigned;
			Assert.IsFalse(defaultId.IsValid);
		}

		[Test]
		public void Constructor_DefaultId_EqualsToUnassigned()
		{
			TrackableID defaultId = new TrackableID();
			Assert.AreEqual(TrackableID.Unassigned, defaultId);
		}

		[Test]
		public void Constructor_NonDefaultId_IsValid()
		{
			TrackableID defaultId = new TrackableID(123);
			Assert.IsTrue(defaultId.IsValid);
		}

		[Test]
		public void Constructor_NonDefaultId_NotEqualToUnassigned()
		{
			TrackableID defaultId = new TrackableID(123);
			Assert.AreNotEqual(TrackableID.Unassigned, defaultId);
		}

		[Test]
		public void ToString_ReturnsExpectedFormat()
		{
			TrackableID trackableID = new TrackableID(123);
			var expected = "TrackableId(123)";

			Assert.AreEqual(expected, trackableID.ToString());
		}

		[Test]
		public void Equals_WithSameID_ReturnsTrue()
		{
			TrackableID id1 = new TrackableID(123);
			TrackableID id2 = new TrackableID(123);

			Assert.IsTrue(id1.Equals(id2));
		}

		[Test]
		public void Equals_WithDifferentID_ReturnsFalse()
		{
			TrackableID id1 = new TrackableID(123);
			TrackableID id2 = new TrackableID(456);

			Assert.IsFalse(id1.Equals(id2));
		}

		[Test]
		public void GetHashCode_SameIDs_ReturnSameHashCode()
		{
			TrackableID id1 = new TrackableID(123);
			TrackableID id2 = new TrackableID(123);

			Assert.AreEqual(id1.GetHashCode(), id2.GetHashCode());
		}

		[Test]
		public void EqualityOperator_WithEqualIDs_ReturnsTrue()
		{
			TrackableID id1 = new TrackableID(123);
			TrackableID id2 = new TrackableID(123);

			Assert.IsTrue(id1 == id2);
		}

		[Test]
		public void InequalityOperator_WithDifferentIDs_ReturnsTrue()
		{
			TrackableID id1 = new TrackableID(123);
			TrackableID id2 = new TrackableID(456);

			Assert.IsTrue(id1 != id2);
		}

		[Test]
		public void TrackableID_SerializesAndDeserializesCorrectly()
		{
			TrackableID original = new TrackableID(123);
			var json = JsonUtility.ToJson(original);
			Debug.Log(json);
			var deserialized = JsonUtility.FromJson<TrackableID>(json);

			// Assert
			Assert.AreEqual(original, deserialized);
		}
	}
}