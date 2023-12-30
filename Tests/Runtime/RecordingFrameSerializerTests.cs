using EventHorizon.Tests.Utilities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EventHorizon.Tests
{
	public class RecordingFrameSerializerTests
	{
		[Test]
		public void FromCurrentFrame_WithEmptyTrackables_ShouldReturnEmptyTrackers()
		{
			// Arrange
			var trackables = new Dictionary<TrackableID, Trackable>();
			var metadata = new RecordingMetadata { fps = new FrameRate(30) }; // Assuming FrameRate and fps are defined
			var frameNumber = 5;

			// Act
			var frameData = RecordingFrameData.FromCurrentFrame(trackables, metadata, frameNumber);

			// Assert
			Assert.IsEmpty(frameData.trackers);
			Assert.AreEqual(frameNumber, frameData.frame);
			Assert.AreEqual(5 * metadata.fps.GetFrameDuration(), frameData.timeCode);
		}


		[Test]
		public void FromCurrentFrame_WithNonEmptyTrackables_ShouldReflectTrackablesData()
		{
			// Arrange
			var trackables = new Dictionary<TrackableID, Trackable>
			{
				{ new TrackableID(1), CreateMockTrackable(new Vector3(1, 1, 1), Quaternion.identity, new Vector3(1, 1, 1)) },
				{ new TrackableID(2), CreateMockTrackable(new Vector3(2, 2, 2), Quaternion.Euler(45, 45, 45), new Vector3(2, 2, 2)) }
			};
			var metadata = new RecordingMetadata { fps = new FrameRate(30) }; // Assuming FrameRate and fps are defined
			var frameNumber = 10;

			// Act
			var frameData = RecordingFrameData.FromCurrentFrame(trackables, metadata, frameNumber);

			// Assert
			Assert.AreEqual(trackables.Count, frameData.trackers.Length);
			for (var i = 0; i < trackables.Count; i++)
			{
				var element = trackables.ElementAt(i);
				Assert.AreEqual(element.Key, frameData.trackers[i].id);
				Assert.AreEqual(element.Value.gameObject.transform.position, frameData.trackers[i].transform.position);
				Assert.AreEqual(element.Value.gameObject.transform.rotation, frameData.trackers[i].transform.rotation);
				Assert.AreEqual(element.Value.gameObject.transform.localScale, frameData.trackers[i].transform.scale);
			}
		}

		[Test]
		public void FromCurrentFrame_WithSpecificFrameNumber_ShouldCalculateTimeCodeCorrectly()
		{
			// Arrange
			var trackables = new Dictionary<TrackableID, Trackable>(); // can be empty or populated
			var metadata = new RecordingMetadata { fps = new FrameRate(60) }; // Assuming FrameRate and fps are defined
			var frameNumber = 120;

			// Act
			var frameData = RecordingFrameData.FromCurrentFrame(trackables, metadata, frameNumber);

			// Assert
			Assert.AreEqual(frameNumber, frameData.frame);
			Assert.AreEqual(120 * metadata.fps.GetFrameDuration(), frameData.timeCode);
		}


		[Test]
		public void FromCurrentFrame_WithMultipleTrackables_ShouldPreserveOrder()
		{
			// Arrange
			var trackables = new Dictionary<TrackableID, Trackable>
			{
				{ new TrackableID(1), CreateMockTrackable(Vector3.zero, Quaternion.identity, Vector3.one) },
				{ new TrackableID(2), CreateMockTrackable(Vector3.one, Quaternion.Euler(45, 45, 45), new Vector3(2, 2, 2)) }
			};
			var metadata = new RecordingMetadata { fps = new FrameRate(30) };
			var frameNumber = 20;

			// Act
			var frameData = RecordingFrameData.FromCurrentFrame(trackables, metadata, frameNumber);

			// Assert
			var trackableIds = trackables.Keys.ToList();
			for (var i = 0; i < trackables.Count; i++)
			{
				Assert.AreEqual(trackableIds[i], frameData.trackers[i].id);
			}
		}

		[Test]
		public void FromCurrentFrame_WithNullTrackable_ThrowsNullReferenceException()
		{
			// Arrange
			var trackables = new Dictionary<TrackableID, Trackable>
			{
				{ new TrackableID(1), null }
			};
			var metadata = new RecordingMetadata { fps = new FrameRate(30) };
			var frameNumber = 10;

			// Act & Assert
			Assert.Throws<NullReferenceException>(() => RecordingFrameData.FromCurrentFrame(trackables, metadata, frameNumber));
		}

		private Trackable CreateMockTrackable(Vector3 position, Quaternion rotation, Vector3 scale)
		{
			var trackable = TrackableTestUtils.CreateTrackable();
			trackable.transform.position = position;
			trackable.transform.rotation = rotation;
			trackable.transform.localScale = scale;
			return trackable;
		}
	}
}