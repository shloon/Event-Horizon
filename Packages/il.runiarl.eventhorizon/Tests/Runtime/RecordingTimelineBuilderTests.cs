using EventHorizon.Tests.Utilities;
using NUnit.Framework;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace EventHorizon.Tests
{
	public class RecordingTimelineBuilderTests
	{
		[Test]
		public void BuildTimeline_WithEmptyRecording_ShouldCreateEmptyTimelineAsset()
		{
			var recording = GenerateEmptyRecordingData();

			RecordingTimelineUtilities.BuildTimeline(recording, out var timelineAsset, out var transformControlTracks);

			Assert.IsNotNull(timelineAsset);
			Assert.AreEqual(0, timelineAsset.outputTrackCount);

			Assert.IsNotNull(transformControlTracks);
			Assert.IsEmpty(transformControlTracks);
		}

		[Test]
		public void BuildTimeline_WithNullRecording_ShouldThrowNullReferenceException() =>
			Assert.Throws<NullReferenceException>(() =>
				RecordingTimelineUtilities.BuildTimeline(null, out var _, out var _));

		[Test]
		public void BuildTimeline_InitializesTimelineAssetWithCorrectFrameRate()
		{
			var recording = GenerateTwoFrameRecordingData();

			RecordingTimelineUtilities.BuildTimeline(recording, out var timelineAsset, out var _);

			Assert.IsNotNull(timelineAsset);
			Assert.AreEqual(recording.metadata.fps.GetAsDouble(), timelineAsset.editorSettings.frameRate);
		}

		[Test]
		public void BuildTimeline_CreatesExpectedNumberOfTransformControlTracks()
		{
			var recording = GenerateTwoFrameRecordingData();

			RecordingTimelineUtilities.BuildTimeline(recording, out var timelineAsset, out var transformControlTracks);

			Assert.AreEqual(trackables.Length, transformControlTracks.Count);
			Assert.AreEqual(trackables.Length, timelineAsset.outputTrackCount);
		}

		[Test]
		public void BuildTimeline_SetsUpClipsWithCorrectDurationInTracks()
		{
			var recording = GenerateTwoFrameRecordingData();

			RecordingTimelineUtilities.BuildTimeline(recording, out var _, out var transformControlTracks);

			foreach (var (_, track) in transformControlTracks)
			{
				Assert.IsTrue(track.hasClips);
				Assert.IsNotEmpty(track.GetClips());

				var clip = track.GetClips().FirstOrDefault();
				Assert.IsNotNull(clip);
				Assert.AreEqual(recording.frames.Length * recording.metadata.fps.GetFrameDuration(), clip.duration);
			}
		}

		[Test]
		public void BuildTimeline_PopulatesTransformControlAssetsWithData()
		{
			var recording = GenerateTwoFrameRecordingData();

			RecordingTimelineUtilities.BuildTimeline(recording, out var _, out var transformControlTracks);

			for (var frameIndex = 0; frameIndex < recording.frames.Length; frameIndex++)
			{
				var frame = recording.frames[frameIndex];
				foreach (var trackableAtFrame in frame.trackers)
				{
					var asset = (TransformControlAsset) transformControlTracks[trackableAtFrame.id].GetClips().First()
						.asset;
					Assert.AreEqual(trackableAtFrame.transform, asset.data[frameIndex],
						"The transform data should match for each trackable in each frame.");
				}
			}
		}

		[Test]
		public void ConfigureDirector_WithEmptyTransformControlTracks_ShouldNotBindAnyAnimations()
		{
			var director = new GameObject("Director").AddComponent<PlayableDirector>();
			var recording = GenerateEmptyRecordingData();
			RecordingTimelineUtilities.BuildTimeline(recording, out var timelineAsset, out var transformControlTracks);

			RecordingTimelineUtilities.ConfigureDirector(transformControlTracks, director, timelineAsset);

			Assert.IsEmpty(transformControlTracks);
			Assert.IsEmpty(director.playableAsset.outputs, "No tracks should be bound to the director.");
		}

		[Test]
		public void ConfigureDirector_WithNullPlayableDirector_ShouldThrowNullReferenceException()
		{
			var recording = GenerateEmptyRecordingData();
			RecordingTimelineUtilities.BuildTimeline(recording, out var timelineAsset, out var transformControlTracks);

			Assert.Throws<NullReferenceException>(() =>
				RecordingTimelineUtilities.ConfigureDirector(transformControlTracks, null, timelineAsset));
		}

		[Test]
		public void ConfigureDirector_WithNullTimelineData_ShouldThrowNullReferenceException()
		{
			var director = new GameObject("Director").AddComponent<PlayableDirector>();

			Assert.Throws<NullReferenceException>(() =>
				RecordingTimelineUtilities.ConfigureDirector(null, director, null));
		}

		[Test]
		public void ConfigureDirector_SetsPlayableDirectorPropertiesCorrectly()
		{
			var director = new GameObject("Director").AddComponent<PlayableDirector>();
			var recording = GenerateEmptyRecordingData();
			RecordingTimelineUtilities.BuildTimeline(recording, out var timelineAsset, out var transformControlTracks);

			RecordingTimelineUtilities.ConfigureDirector(transformControlTracks, director, timelineAsset);

			Assert.AreEqual(false, director.playOnAwake);
			Assert.AreEqual(timelineAsset, director.playableAsset);
		}

		[Test]
		public void ConfigureDirector_BindsAnimationTracksToCorrectAnimators()
		{
			var director = new GameObject("Director").AddComponent<PlayableDirector>();
			var recording = GenerateTwoFrameRecordingData();
			RecordingTimelineUtilities.BuildTimeline(recording, out var timelineAsset, out var transformControlTracks);

			RecordingTimelineUtilities.ConfigureDirector(transformControlTracks, director, timelineAsset);

			foreach (var (id, trackable) in TrackableManagerComponent.Instance.RegisteredTrackables)
			{
				if (trackable is TrackableComponent trackableComponent)
				{
					var animator = trackableComponent.GetComponent<Animator>();
					var binding = director.GetGenericBinding(transformControlTracks[id]);
					Assert.AreEqual(animator, binding);
				}
			}
		}

		#region SetupTearDown

		private TrackableComponent[] trackables;
		private TrackableManagerComponent trackableManager;

		[UnitySetUp]
		public IEnumerator Setup()
		{
			trackableManager = new GameObject("Trackable Manager").AddComponent<TrackableManagerComponent>();

			const int NUM_TRACKABLES = 16;
			trackables = new TrackableComponent[NUM_TRACKABLES];
			for (var i = 0; i < trackables.Length; i++)
			{
				trackables[i] = TrackableTestUtils.CreateTrackableGameObject(trackableManager);
			}

			yield return null;
		}

		[UnityTearDown]
		public IEnumerator TearDown()
		{
			foreach (var trackable in trackables)
			{
				TrackableTestUtils.DestroyTrackableGameObject(trackable);
			}

			Object.Destroy(trackableManager);
			yield return null;
		}

		#endregion

		#region Utilities

		private RecordingData GenerateEmptyRecordingData()
		{
			var data = new RecordingData();
			data.metadata.sceneName = "Test";
			data.metadata.fps = new FrameRate(1);
			return data;
		}

		private RecordingData GenerateTwoFrameRecordingData()
		{
			var data = GenerateEmptyRecordingData();
			data.metadata.sceneName = "Test";
			data.metadata.fps = new FrameRate(1);
			data.frames = new[]
			{
				RecordingFrameData.FromCurrentFrame(trackableManager.RegisteredTrackables, in data.metadata, 0),
				RecordingFrameData.FromCurrentFrame(trackableManager.RegisteredTrackables, in data.metadata, 1)
			};

			return data;
		}

		#endregion
	}
}