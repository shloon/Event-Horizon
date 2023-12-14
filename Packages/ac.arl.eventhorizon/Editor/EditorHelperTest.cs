using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace EventHorizon.Editor
{
	// we use SessionState since we want the toggle value to persist
	// and InitializeOnLoad is called whenever switching to either play or edit mode.
	[InitializeOnLoad]
	public abstract class EventHorizonInspectionStateToggler
	{
		private const string ToggleKeyString = "isCurrentlyInspectingEventHorizon";

		static EventHorizonInspectionStateToggler() => EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

		[MenuItem("EH/Inspect File")]
		public static void TogglePlayModeAndSetupCleanup()
		{
			Debug.Log($"initiatedByToggleButton changed state to {!EditorApplication.isPlaying}");
			SessionState.SetBool(ToggleKeyString, !EditorApplication.isPlaying);
			ToggleRecorder(EditorApplication.isPlaying);

			EditorApplication.isPlaying = !EditorApplication.isPlaying;
		}

		private static void ToggleRecorder(bool shouldRecord)
		{
			var recorder = TrackableManagerComponent.Instance.GetComponent<RecorderComponent>();
			if (recorder)
				recorder.enabled = shouldRecord;
		}

		private static void OnPlayModeStateChanged(PlayModeStateChange stateChange)
		{
			switch (stateChange)
			{
				case PlayModeStateChange.EnteredPlayMode:
					if (SessionState.GetBool(ToggleKeyString, false))
					{
						OnEnteredPlayMode();
					}

					break;
				case PlayModeStateChange.ExitingPlayMode:
					// No need for a separate flag, the action will be triggered when exiting play mode
					break;
				case PlayModeStateChange.EnteredEditMode:
					if (SessionState.GetBool(ToggleKeyString, false))
					{
						OnExitingPlayMode();
					}
					SessionState.SetBool(ToggleKeyString, false);
					break;
			}
		}

		private static void OnEnteredPlayMode()
		{
			Debug.Log("Now in Play Mode - performing actions (initiated by toggle button).");
			var recorder = TrackableManagerComponent.Instance.GetComponent<RecorderComponent>();
			if (recorder)
				recorder.enabled = false;

			// read recording
			var recording = RecordingDataUtilities.Load("Assets/Recordings/recording.evh");

			// build timeline
			var timeline = ScriptableObject.CreateInstance<TimelineAsset>();
			timeline.editorSettings.frameRate = recording.metadata.fps.GetAsDouble();

			// build timeline data
			var transformControlTracks = new Dictionary<TrackableID, TransformControlTrack>();
			var transformControlAssets = new Dictionary<TrackableID, TransformControlAsset>();
			var trackableIDs = recording.frames.SelectMany(frame => frame.trackers.Select(x => x.id)).Distinct().ToList();

			foreach (var id in trackableIDs)
			{
				var track = timeline.CreateTrack<TransformControlTrack>();
				transformControlTracks.Add(id, track);

				var clip = track.CreateClip<TransformControlAsset>();
				clip.duration = recording.frames.Length * recording.metadata.fps.GetFrameDuration();

				var asset = ((TransformControlAsset) clip.asset);
				asset.data = new TransformData[recording.frames.Length];
				asset.metadata = recording.metadata;
				transformControlAssets.Add(id, asset);
			}

			for (var frameIndex = 0; frameIndex < recording.frames.Length; frameIndex++)
			{
				foreach (var trackable in recording.frames[frameIndex].trackers)
				{
					transformControlAssets[trackable.id].data[frameIndex] = trackable.transform;
				}
			}

			// add director
			var director = TrackableManagerComponent.Instance.gameObject.AddComponent<PlayableDirector>();
			director.playOnAwake = false;
			director.playableAsset = timeline;

			// connect timeline and objects
			foreach (var (id, trackable) in TrackableManagerComponent.Instance.RegisteredTrackables)
			{
				var animator = trackable.gameObject.AddComponent<Animator>();
				var animationTrack = transformControlTracks[id];
				director.SetGenericBinding(animationTrack, animator);
			}

			// focus on gameobject and timeline editor window
			EditorWindow.GetWindow<UnityEditor.SceneView>().Focus();
			Selection.activeObject = timeline;
			Selection.activeGameObject = director.gameObject;
			EditorWindow.GetWindow<UnityEditor.Timeline.TimelineEditorWindow>().Focus();
		}

		private static void OnExitingPlayMode()
		{
			Debug.Log("Exiting Play Mode - performing cleanup (initiated by toggle button).");
			ToggleRecorder(true);
		}
	}
}