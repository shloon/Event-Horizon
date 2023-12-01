using NUnit.Framework;
using System;
using UnityEngine;

namespace EventHorizon.Tests
{
	// TODO: serialize/deserialize memory
	[Parallelizable]
	public class RecordingRoundtripTests
	{
		private const string EMPTY_RECORDING_PATH =
			"Packages/com.ac.arl.eventhorizon/Tests/Runtime/Recordings/EmptyRecording.evh";

		private const string FRAMES_BUT_ZERO_TRACKABLES_RECORDING_PATH =
			"Packages/com.ac.arl.eventhorizon/Tests/Runtime/Recordings/FramesButZeroTrackablesRecording.evh";

		private const string SINGLE_TRACKABLE_RECORDING_PATH =
			"Packages/com.ac.arl.eventhorizon/Tests/Runtime/Recordings/SingleTrackableRecording.evh";

		private const string MULTI_TRACKABLE_RECORDING_PATH =
			"Packages/com.ac.arl.eventhorizon/Tests/Runtime/Recordings/MultiTrackableRecording.evh";

		private const int NUM_MULTIPLE_TRACKABLES = 100;
		private const int NUM_FRAMES = 100;

		private static readonly RecordingMetadata METADATA =
			new RecordingMetadata() { fps = new FrameRate(1), sceneName = "Test" };

		private static readonly Vector3 POSITION_DATA = new Vector3(1, 2, 3);
		private static readonly Quaternion ROTATION_DATA = new Quaternion(4, 5, 6, 7);
		private static readonly Vector3 SCALE_DATA = new Vector3(8, 9, 0);

		private static readonly TransformData TRANSFORM_DATA =
			new() { position = POSITION_DATA, rotation = ROTATION_DATA, scale = SCALE_DATA };

		[Test, Ignore("make test")]
		public void MakeEmptyRecording()
		{
			Recording r = new Recording(EMPTY_RECORDING_PATH, METADATA);
			r.WriteHeader();
			r.WrapStream();
		}

		[Test]
		public void DecodeEmptyRecording()
		{
			var data = System.IO.File.ReadAllBytes(EMPTY_RECORDING_PATH);
			var recording = RecordingDataUtilities.Load(data);
			Assert.AreEqual(RecordingFormatVersion.V1, recording.version);
			Assert.AreEqual(METADATA.fps, recording.metadata.fps);
			Assert.AreEqual(METADATA.sceneName, recording.metadata.sceneName);
		}

		[Test, Ignore("make test")]
		public void MakeFramesButZeroTrackablesRecording()
		{
			Recording r = new Recording(FRAMES_BUT_ZERO_TRACKABLES_RECORDING_PATH,
				METADATA);

			r.WriteHeader();
			for (var i = 0; i < NUM_FRAMES; ++i)
			{
				r.WriteFrame(new RecordingFrameData
				{
					frame = i, timeCode = i, trackers = Array.Empty<RecordingTrackerData>()
				});
			}

			r.WrapStream();
		}

		[Test]
		public void DecodeFramesButZeroTrackablesRecording()
		{
			var data = System.IO.File.ReadAllBytes(FRAMES_BUT_ZERO_TRACKABLES_RECORDING_PATH);
			var recording = RecordingDataUtilities.Load(data);
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

		[Test, Ignore("make test")]
		public void MakeSingleTrackableRecording()
		{
			Recording r = new Recording(SINGLE_TRACKABLE_RECORDING_PATH,
				METADATA);

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
		}

		[Test]
		public void DecodeSingleTrackableRecording()
		{
			var data = System.IO.File.ReadAllBytes(SINGLE_TRACKABLE_RECORDING_PATH);
			var recording = RecordingDataUtilities.Load(data);
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

		[Test, Ignore("make test")]
		public void MakeMultiTrackableRecording()
		{
			Recording r = new Recording(MULTI_TRACKABLE_RECORDING_PATH,
				METADATA);
			RecordingFrameData frameData = new RecordingFrameData
			{
				frame = 0, timeCode = 0, trackers = new RecordingTrackerData[NUM_MULTIPLE_TRACKABLES]
			};

			for (var i = 0; i < NUM_MULTIPLE_TRACKABLES; ++i)
			{
				frameData.trackers[i] = new RecordingTrackerData
				{
					id = new TrackableID((uint) i),
					transform = new TransformData()
					{
						position = POSITION_DATA, rotation = ROTATION_DATA, scale = SCALE_DATA
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
		}

		[Test]
		public void DecodeMultiTrackableRecording()
		{
			var data = System.IO.File.ReadAllBytes(MULTI_TRACKABLE_RECORDING_PATH);
			var recording = RecordingDataUtilities.Load(data);
			Assert.AreEqual(RecordingFormatVersion.V1, recording.version);
			Assert.AreEqual(METADATA.fps, recording.metadata.fps);
			Assert.AreEqual(METADATA.sceneName, recording.metadata.sceneName);

			for (var i = 0; i < NUM_FRAMES; ++i)
			{
				var frameData = recording.frames[i];

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