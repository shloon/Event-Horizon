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
			var trackableID = new TrackableID(id);

			Assert.AreEqual(id, trackableID.Internal);
		}

		[Test]
		public void Constructor_DefaultId_IsNotValid()
		{
			var defaultId = TrackableID.Unassigned;
			Assert.IsFalse(defaultId.IsValid);
		}

		[Test]
		public void Constructor_DefaultId_EqualsToUnassigned()
		{
			var defaultId = new TrackableID();
			Assert.AreEqual(TrackableID.Unassigned, defaultId);
		}

		[Test]
		public void Constructor_NonDefaultId_IsValid()
		{
			var defaultId = new TrackableID(123);
			Assert.IsTrue(defaultId.IsValid);
		}

		[Test]
		public void Constructor_NonDefaultId_NotEqualToUnassigned()
		{
			var defaultId = new TrackableID(123);
			Assert.AreNotEqual(TrackableID.Unassigned, defaultId);
		}

		[Test]
		public void ToString_ReturnsExpectedFormat()
		{
			var trackableID = new TrackableID(123);
			var expected = "TrackableId(123)";

			Assert.AreEqual(expected, trackableID.ToString());
		}

		[Test]
		public void Equals_WithSameID_ReturnsTrue()
		{
			var id1 = new TrackableID(123);
			var id2 = new TrackableID(123);

			Assert.IsTrue(id1.Equals(id2));
		}

		[Test]
		public void Equals_WithDifferentID_ReturnsFalse()
		{
			var id1 = new TrackableID(123);
			var id2 = new TrackableID(456);

			Assert.IsFalse(id1.Equals(id2));
		}

		[Test]
		public void GetHashCode_SameIDs_ReturnSameHashCode()
		{
			var id1 = new TrackableID(123);
			var id2 = new TrackableID(123);

			Assert.AreEqual(id1.GetHashCode(), id2.GetHashCode());
		}

		[Test]
		public void EqualityOperator_WithEqualIDs_ReturnsTrue()
		{
			var id1 = new TrackableID(123);
			var id2 = new TrackableID(123);

			Assert.IsTrue(id1 == id2);
		}

		[Test]
		public void InequalityOperator_WithDifferentIDs_ReturnsTrue()
		{
			var id1 = new TrackableID(123);
			var id2 = new TrackableID(456);

			Assert.IsTrue(id1 != id2);
		}

		[Test]
		public void TrackableID_SerializesAndDeserializesCorrectly()
		{
			var original = new TrackableID(123);
			var json = JsonUtility.ToJson(original);
			Debug.Log(json);
			var deserialized = JsonUtility.FromJson<TrackableID>(json);

			// Assert
			Assert.AreEqual(original, deserialized);
		}
	}
}