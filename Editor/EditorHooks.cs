using EventHorizon.Editor.ProblemSolver;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

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
	}
}