using EventHorizon.Editor.RecordingsV2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace EventHorizon
{
	public static class RecordingTimelineUtilities
	{
		public static TimelineData BuildTimeline(FormatV2Scriptable formatV2Data)
		{
			var timelineData = new TimelineData { timelineAsset = ScriptableObject.CreateInstance<TimelineAsset>() };
			var frameDuration = formatV2Data.metadataPacket.fps.GetFrameDuration();
			timelineData.timelineAsset.editorSettings.frameRate = formatV2Data.metadataPacket.fps.GetAsDouble();

			// handle edge case of no actual data
			if (formatV2Data.framePackets.Count == 0 && formatV2Data.transformPackets.Count == 0 &&
				formatV2Data.genericDataPackets.Count == 0 && formatV2Data.activationPackets.Count == 0)
			{
				return null;
			}

			// setup tracks for transform-related information
			// note we don't have to sort the packets by frame since they're serialized in-order.
			if (formatV2Data.framePackets.Count > 0)
			{
				BuildTransformTracks(formatV2Data, frameDuration, timelineData);
			}

			if (formatV2Data.activationPackets.Count > 0)
			{
				BuildActivationTracks(formatV2Data, frameDuration, timelineData);
			}

			// setup a generic track for generic trackable
			if (formatV2Data.genericDataPackets.Count > 0)
			{
				BuildGenericDataTrack(timelineData, frameDuration);
			}

			return timelineData;
		}

		private static void BuildGenericDataTrack(TimelineData timelineData, double frameDuration)
		{
			timelineData.genericDataTrack = timelineData.timelineAsset.CreateTrack<GenericDataControlTrack>();
			var genericDataClip = timelineData.genericDataTrack.CreateClip<GenericDataControlAsset>();
			((GenericDataControlAsset)genericDataClip.asset).frameDuration = frameDuration;

			// TODO, currently not on MVP
		}

		private static void BuildTransformTracks(FormatV2Scriptable formatV2Data, double frameDuration,
			TimelineData timelineData)
		{
			Dictionary<TrackableID, TimelineClip> transformClips = new();
			Dictionary<TrackableID, TransformControlAsset> transformAssets = new();
			Dictionary<TrackableID, ulong> transformClipsFrames = new();

			foreach (var packet in formatV2Data.transformPackets)
			{
				var trackableID = new TrackableID(packet.id);
				var packetTime = packet.frame * frameDuration;

				// Create a new clip if it doesn't exist or there is a gap of at least one frame
				if (!transformClips.TryGetValue(trackableID, out var clip) ||
					packet.frame > transformClipsFrames[trackableID] + 1)
				{
					if (!timelineData.transformTracks.TryGetValue(trackableID, out var track))
					{
						track = timelineData.timelineAsset.CreateTrack<TransformControlTrack>();
						timelineData.transformTracks[trackableID] = track;
					}

					clip = track.CreateClip<TransformControlAsset>();
					clip.duration = 0;
					clip.start = packetTime;
					transformClips[trackableID] = clip;

					transformAssets[trackableID] = (TransformControlAsset)clip.asset;
					transformAssets[trackableID].frameDuration = frameDuration;
				}

				// Update clip duration
				clip.duration = packetTime - clip.start;
				transformClipsFrames[trackableID] = packet.frame;

				// Append frame data to the clip
				transformAssets[trackableID].packets.Add(packet);
			}
		}

		private static void BuildActivationTracks(FormatV2Scriptable formatV2Data, double frameDuration,
			TimelineData timelineData)
		{
			Dictionary<TrackableID, TimelineClip> activationClips = new();
			Dictionary<TrackableID, ulong> activationClipFrames = new();

			foreach (var packet in formatV2Data.activationPackets)
			{
				var trackableID = new TrackableID(packet.id);
				var packetTime = packet.frame * frameDuration;

				// Create a new clip if it doesn't exist or there is a gap of at least one frame
				if (!activationClips.TryGetValue(trackableID, out var clip) ||
					packet.frame > activationClipFrames[trackableID] + 1)
				{
					if (!timelineData.activationTracks.TryGetValue(trackableID, out var track))
					{
						track = timelineData.timelineAsset.CreateTrack<ActivationTrack>();
						track.postPlaybackState = ActivationTrack.PostPlaybackState.Inactive;
						timelineData.activationTracks[trackableID] = track;
					}

					clip = track.CreateDefaultClip();
					clip.duration = 0;
					clip.start = packetTime;
					activationClips[trackableID] = clip;
				}

				// Update clip duration
				clip.duration = packetTime - clip.start;
				activationClipFrames[trackableID] = packet.frame;
			}
		}

		public static void ConfigureDirector(in TimelineData timelineData, PlayableDirector director,
			out List<GameObject> boundGameObjects)
		{
			if (timelineData == null || director == null)
			{
				boundGameObjects = null;
				return;
			}

			director.playOnAwake = false;
			director.playableAsset = timelineData.timelineAsset;

			boundGameObjects = new List<GameObject>();

			// find all trackable components, regardless of their active state
			var transformTrackables =
				Object.FindObjectsByType<TransformTrackableComponent>(FindObjectsInactive.Include,
					FindObjectsSortMode.None);
			foreach (var trackableComponent in transformTrackables)
			{
				var animator = trackableComponent.gameObject.AddComponent<Animator>();
				if (timelineData.transformTracks.TryGetValue(trackableComponent.Id, out var transformControlTrack))
				{
					director.SetGenericBinding(transformControlTrack, animator);
					boundGameObjects.Add(trackableComponent.gameObject);
				}
			}

			// force-bind to all activation-trackable components in scene
			var activationTrackables =
				Object.FindObjectsByType<ActivationTrackableComponent>(FindObjectsInactive.Include,
					FindObjectsSortMode.None);
			foreach (var trackableComponent in activationTrackables)
			{
				if (timelineData.activationTracks.TryGetValue(trackableComponent.Id, out var activationTrack))
				{
					director.SetGenericBinding(activationTrack, trackableComponent.gameObject);
					boundGameObjects.Add(trackableComponent.gameObject);
				}
			}
		}


		public static void ToggleGameObjects(List<GameObject> boundGameObjects, bool shouldEnable)
		{
			foreach (var gameObject in boundGameObjects)
			{
				var rigidbody = gameObject.GetComponent<Rigidbody>();
				if (rigidbody != null)
				{
					rigidbody.isKinematic = !shouldEnable;
				}
			}
		}

		public class TimelineData
		{
			public Dictionary<TrackableID, ActivationTrack> activationTracks = new();
			public GenericDataControlTrack genericDataTrack;
			public TimelineAsset timelineAsset;
			public Dictionary<TrackableID, TransformControlTrack> transformTracks = new();
		}
	}
}