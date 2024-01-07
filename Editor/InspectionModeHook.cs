using EventHorizon.Editor.RecordingsV2;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Playables;

namespace EventHorizon.Editor
{
	[InitializeOnLoad]
	public class InspectionModeHook
	{
		public delegate void InspectionModeStartHandler();

		// we use SessionState since we want the toggle value to persist
		// and InitializeOnLoad is called whenever switching to either play or edit mode.
		private const string ToggleKeyString = "isCurrentlyInspectingEventHorizon";

		[MenuItem("Event Horizon/Play Selected Recording")]
		public static void TogglePlayModeAndSetupCleanup() => TogglePlaymode(true);

		public static event InspectionModeStartHandler onInspectionModeStart;

		private static void ToggleRecorder(bool shouldRecord)
		{
			var recorder = TrackableManagerComponent.Instance.GetComponent<RecorderComponent>();
			if (recorder)
			{
				recorder.enabled = shouldRecord;
			}
		}

		private static void OnEnteredPlayMode()
		{
			Debug.Log("Starting EVH file playback mode...");

			var serializedFormatV2 = Selection.activeObject as FormatV2Scriptable;
			if (!serializedFormatV2)
			{
				Debug.LogError("No recording selected, aborting...");
				return;
			}

			var recorder = TrackableManagerComponent.Instance.GetComponent<RecorderComponent>();
			if (recorder)
			{
				recorder.enabled = false;
			}

			onInspectionModeStart?.Invoke();

			// read recording
			var director = TrackableManagerComponent.Instance.gameObject.AddComponent<PlayableDirector>();

			// build and configure timeline
			var timelineData = RecordingTimelineUtilities.BuildTimeline(serializedFormatV2);
			RecordingTimelineUtilities.ConfigureDirector(timelineData, director, out var boundGameObjects);

			// disable any physics or similar things on game objects
			RecordingTimelineUtilities.ToggleGameObjects(boundGameObjects, false);

			// focus on gameobject and timeline editor window
			EditorWindow.GetWindow<SceneView>().Focus();
			Selection.activeObject = timelineData.timelineAsset;
			Selection.activeGameObject = director.gameObject;
			EditorWindow.GetWindow<TimelineEditorWindow>().Focus();

			// TODO: Dock timeline window
		}

		private static void OnExitingPlayMode()
		{
			Debug.Log("Stopping EVH file playback mode...");
			ToggleRecorder(true);
		}

		#region Playmode Toggle Logic

		static InspectionModeHook() => EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

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

		#endregion
	}
}