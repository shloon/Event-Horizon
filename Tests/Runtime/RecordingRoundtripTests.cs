using NUnit.Framework;
using System;
using System.IO;
using UnityEngine;

namespace EventHorizon.Tests
{
	// TODO: serialize/deserialize memory
	public class RecordingRoundtripTests
	{
		private const int NUM_MULTIPLE_TRACKABLES = 16;
		private const int NUM_FRAMES = 16 * 16;

		private static readonly RecordingMetadata METADATA = new() { fps = new FrameRate(1), sceneName = "Test" };

		private static readonly Vector3 POSITION_DATA = new(1, 2, 3);
		private static readonly Quaternion ROTATION_DATA = new(4, 5, 6, 7);
		private static readonly Vector3 SCALE_DATA = new(8, 9, 0);

		private static readonly TransformData TRANSFORM_DATA =
			new() { position = POSITION_DATA, rotation = ROTATION_DATA, scale = SCALE_DATA };

		[Test]
		public void MakeEmptyRecording()
		{
			using var stream = new MemoryStream();
			var r = new RecordingWriter(stream, METADATA);
			r.WriteHeader();
			r.WrapStream();
			r.Close();

			var recording = RecordingDataUtilities.Load(stream);
			Assert.AreEqual(RecordingFormatVersion.V1, recording.version);
			Assert.AreEqual(METADATA.fps, recording.metadata.fps);
			Assert.AreEqual(METADATA.sceneName, recording.metadata.sceneName);
		}

		[Test]
		public void MakeFramesButZeroTrackablesRecording()
		{
			using var stream = new MemoryStream();
			var r = new RecordingWriter(stream, METADATA);
			r.WriteHeader();
			for (var i = 0; i < NUM_FRAMES; ++i)
			{
				r.WriteFrame(new RecordingFrameData
				{
					frame = i,
					timeCode = i,
					trackers = Array.Empty<RecordingTrackerData>()
				});
			}

			r.WrapStream();
			r.Close();

			var recording = RecordingDataUtilities.Load(stream);
			Assert.AreEqual(RecordingFormatVersion.V1, recording.version);
			Assert.AreEqual(METADATA.fps, recording.metadata.fps);
			Assert.AreEqual(METADATA.sceneName, recording.metadata.sceneName);

			for (var i = 0; i < NUM_FRAMES; ++i)
			{
				var frameData = recording.frames[i];
				Assert.AreEqual(i, frameData.frame);
				Assert.AreEqual(i, frameData.timeCode);
				Assert.IsEmpty(frameData.trackers);
			}
		}

		[Test]
		public void MakeSingleTrackableRecording()
		{
			using var stream = new MemoryStream();
			var r = new RecordingWriter(stream, METADATA);
			r.WriteHeader();
			for (var i = 0; i < NUM_FRAMES; ++i)
			{
				var frameData = new RecordingFrameData
				{
					frame = i,
					timeCode = i,
					trackers = new[]
					{
						new RecordingTrackerData { id = new TrackableID(1), transform = TRANSFORM_DATA }
					}
				};
				r.WriteFrame(frameData);
			}

			r.WrapStream();
			r.Close();

			var recording = RecordingDataUtilities.Load(stream);
			Assert.AreEqual(RecordingFormatVersion.V1, recording.version);
			Assert.AreEqual(METADATA.fps, recording.metadata.fps);
			Assert.AreEqual(METADATA.sceneName, recording.metadata.sceneName);

			for (var i = 0; i < NUM_FRAMES; ++i)
			{
				var frameData = recording.frames[i];

				Assert.AreEqual(i, frameData.frame);
				Assert.AreEqual(i, frameData.timeCode);
				Assert.IsNotEmpty(frameData.trackers);

				var trackerData = frameData.trackers[0];
				Assert.AreEqual(new TrackableID(1), trackerData.id);
				Assert.AreEqual(POSITION_DATA, trackerData.transform.position);
				Assert.AreEqual(ROTATION_DATA, trackerData.transform.rotation);
				Assert.AreEqual(SCALE_DATA, trackerData.transform.scale);
			}
		}

		[Test]
		public void MakeMultiTrackableRecording()
		{
			using var stream = new MemoryStream();
			var r = new RecordingWriter(stream, METADATA);
			var frameData = new RecordingFrameData
			{
				frame = 0,
				timeCode = 0,
				trackers = new RecordingTrackerData[NUM_MULTIPLE_TRACKABLES]
			};
			for (var i = 0; i < NUM_MULTIPLE_TRACKABLES; ++i)
			{
				frameData.trackers[i] = new RecordingTrackerData
				{
					id = new TrackableID((uint) i),
					transform = new TransformData
					{
						position = POSITION_DATA,
						rotation = ROTATION_DATA,
						scale = SCALE_DATA
					}
				};
			}

			r.WriteHeader();
			for (var i = 0; i < NUM_FRAMES; ++i)
			{
				frameData.frame = i;
				frameData.timeCode = i;
				r.WriteFrame(frameData);
			}

			r.WrapStream();
			r.Close();

			var recording = RecordingDataUtilities.Load(stream);
			Assert.AreEqual(RecordingFormatVersion.V1, recording.version);
			Assert.AreEqual(METADATA.fps, recording.metadata.fps);
			Assert.AreEqual(METADATA.sceneName, recording.metadata.sceneName);

			for (var i = 0; i < NUM_FRAMES; ++i)
			{
				frameData = recording.frames[i];

				Assert.AreEqual(i, frameData.frame);
				Assert.AreEqual(i, frameData.timeCode);
				Assert.IsNotEmpty(frameData.trackers);

				for (var j = 0; j < NUM_MULTIPLE_TRACKABLES; ++j)
				{
					var trackerData = frameData.trackers[j];
					Assert.AreEqual(new TrackableID((uint) j), trackerData.id);
					Assert.AreEqual(POSITION_DATA, trackerData.transform.position);
					Assert.AreEqual(ROTATION_DATA, trackerData.transform.rotation);
					Assert.AreEqual(SCALE_DATA, trackerData.transform.scale);
				}
			}
		}
	}
}