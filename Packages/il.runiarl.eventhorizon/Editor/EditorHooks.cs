using EventHorizon.Editor.ProblemSolver;
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EventHorizon.Editor
{
	// class for all our editor hooks
	public static class EditorHooks
	{
		private static readonly AggregateProblemSolver Solver = new();

		[InitializeOnLoadMethod]
		private static void TrackableSceneSaveHelperHook()
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode)
			{
				return;
			}

			Debug.Log("EventHorizon: Added onSaving hook");
			EditorSceneManager.sceneSaving += (scene, _) => Solver.DiscoverProblems(scene);
		}

		[MenuItem("Event Horizon/Fix All Issues")]
		public static void AutoFixAllIssues() => Solver.DiscoverProblems(SceneManager.GetActiveScene());

		[MenuItem("GameObject/EventHorizon/Trackable Manager", priority = 10)]
		public static void CreateTrackableManagerGameObject()
		{
			try
			{
				if (TrackableManagerComponent.Instance != null)
				{
					Debug.LogWarning("TrackableManager already exists in scene");
				}
			}
			catch (NullReferenceException)
			{
				var trackableManagerGO = new GameObject("Trackable Manager");
				trackableManagerGO.AddComponent<TrackableManagerComponent>();
				trackableManagerGO.AddComponent<RecorderComponent>();
			}
		}
	}
}