using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

namespace EventHorizon.Editor
{
	// we use SessionState since we want the toggle value to persist
	// and InitializeOnLoad is called whenever switching to either play or edit mode.
	[InitializeOnLoad]
	public abstract class EventHorizonInspectionStateToggler
	{
		private const string ToggleKeyString = "isCurrentlyInspectingEventHorizon";

		#region Playmode Toggle Logic
		static EventHorizonInspectionStateToggler() => EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

		private static void TogglePlaymode(bool startPlaing)
		{
			Debug.Log($"initiatedByToggleButton changed state to {startPlaing}");
			SessionState.SetBool(ToggleKeyString, startPlaing);
			ToggleRecorder(!startPlaing);
			EditorApplication.isPlaying = startPlaing;
		}

		private static void OnPlayModeStateChanged(PlayModeStateChange stateChange)
		{
			switch (stateChange)
			{
				case PlayModeStateChange.EnteredPlayMode:
					if (SessionState.GetBool(ToggleKeyString, false))
						OnEnteredPlayMode();

					break;

				case PlayModeStateChange.ExitingPlayMode:
					// No need for a separate flag, the action will be triggered when exiting play mode
					break;

				case PlayModeStateChange.EnteredEditMode:
					if (SessionState.GetBool(ToggleKeyString, false))
						OnExitingPlayMode();

					SessionState.SetBool(ToggleKeyString, false);
					break;
			}
		}

		#endregion

		public static void TogglePlayModeAndSetupCleanup()
		{
			TogglePlaymode(true);
		}

		private static void ToggleRecorder(bool shouldRecord)
		{
			var recorder = TrackableManagerComponent.Instance.GetComponent<RecorderComponent>();
			if (recorder)
				recorder.enabled = shouldRecord;
		}

		private static void OnEnteredPlayMode()
		{
			Debug.Log("Now in Play Mode - performing actions (initiated by toggle button).");
			StartTimeline();
		}

		private static void StartTimeline()
		{
			var recordingDataObject = Selection.activeObject as RecordingDataScriptable;
			if (!recordingDataObject)
			{
				Debug.LogError("No recording selected, aborting...");
				return;
			}

			var recorder = TrackableManagerComponent.Instance.GetComponent<RecorderComponent>();
			if (recorder)
				recorder.enabled = false;

			// read recording
			var recording = recordingDataObject.data;
			var director = TrackableManagerComponent.Instance.gameObject.AddComponent<PlayableDirector>();

			// build and configure timeline
			RecordingTimelineUtilities.BuildTimeline(recording, out var timelineAsset, out var transformControlTracks);
			RecordingTimelineUtilities.ConfigureDirector(transformControlTracks, director, timelineAsset);

			// focus on gameobject and timeline editor window
			EditorWindow.GetWindow<SceneView>().Focus();
			Selection.activeObject = timelineAsset;
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