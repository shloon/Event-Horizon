using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace EventHorizon
{
	public static class RecordingTimelineUtilities
	{
		public static void BuildTimeline(RecordingData recording, out TimelineAsset timelineAsset,
			out Dictionary<TrackableID, TransformControlTrack> transformControlTracks)
		{
			// build timeline
			timelineAsset = ScriptableObject.CreateInstance<TimelineAsset>();
			timelineAsset.editorSettings.frameRate = recording.metadata.fps.GetAsDouble();

			// build timeline data
			transformControlTracks = new Dictionary<TrackableID, TransformControlTrack>();
			if (recording.frames == null || recording.frames.Length == 0)
			{
				return;
			}

			var transformControlAssets = new Dictionary<TrackableID, TransformControlAsset>();
			var trackableIDs = recording.frames.SelectMany(frame => frame.trackers.Select(x => x.id))
				.Distinct()
				.ToList();

			// setup 
			foreach (var id in trackableIDs)
			{
				var track = timelineAsset.CreateTrack<TransformControlTrack>();
				transformControlTracks.Add(id, track);

				var clip = track.CreateClip<TransformControlAsset>();
				clip.duration = recording.frames.Length * recording.metadata.fps.GetFrameDuration();

				var asset = (TransformControlAsset) clip.asset;
				asset.data = new TransformData[recording.frames.Length];
				asset.metadata = recording.metadata;
				transformControlAssets.Add(id, asset);
			}

			// setup transform data
			for (var frameIndex = 0; frameIndex < recording.frames.Length; frameIndex++)
			{
				foreach (var trackable in recording.frames[frameIndex].trackers)
				{
					transformControlAssets[trackable.id].data[frameIndex] = trackable.transform;
				}
			}
		}

		public static void ConfigureDirector(Dictionary<TrackableID, TransformControlTrack> transformControlTracks,
			PlayableDirector director, TimelineAsset timelineAsset)
		{
			director.playOnAwake = false;
			director.playableAsset = timelineAsset;

			foreach (var (id, trackable) in TrackableManagerComponent.Instance.RegisteredTrackables)
			{
				if (trackable is TrackableComponent trackableComponent)
				{
					var animator = trackableComponent.gameObject.AddComponent<Animator>();
					if (transformControlTracks.TryGetValue(id, out var animationTrack))
					{
						director.SetGenericBinding(animationTrack, animator);
					}
				}
			}
		}
	}
}