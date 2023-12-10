using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Scene = UnityEngine.SceneManagement.Scene;

namespace EventHorizon.Editor
{
	public static class TrackableSceneSaveHelper
	{
		public static void ValidateSceneTrackableIds(Scene scene, bool fix = true)
		{
			var trackableIDManager = new TrackableManager();
			
			// simulate a game scene, and try finding and fixing problems
			// TODO: should we refactor this to a warning-detection and warning-fixing method for future extendibility?
			var rootGOs = scene.GetRootGameObjects();
			foreach (var go in rootGOs)
			{
				var components = go.GetComponentsInChildren<Trackable>();
				foreach (var trackable in components)
				{
					if (trackable.id == TrackableID.Unassigned)
					{
						if (fix)
						{
							trackable.id = trackableIDManager.GenerateId();
							Debug.LogWarning($"EventHorizon: Fixed: GameObject \"{trackable.name}\": {trackable.id} has no id assigned to it.");
						}
						else
						{
							Debug.LogWarning($"EventHorizon: GameObject \"{trackable.name}\": {trackable.id} has no id assigned to it.");
						}
					}
					else if (trackableIDManager.RegisteredTrackables.ContainsKey(trackable.id))
					{
						if (fix)
						{
							trackable.id = trackableIDManager.GenerateId();
							Debug.LogWarning($"EventHorizon: Fixed: GameObject \"{trackable.name}\": {trackable.id} had existing TrackableID.");
						}
						else
						{
							Debug.LogWarning($"EventHorizon: GameObject \"{trackable.name}\": {trackable.id} had existing TrackableID.");
						}
					}

					trackableIDManager.Register(trackable);
				}
			}
		}
	}
	
	// Hook wrapper class that actually registers and properly calls this validation
	[InitializeOnLoad]
	public static class TrackableSceneSaveHelperHook
	{
		static TrackableSceneSaveHelperHook()
		{
			Debug.Log("EventHorizon: Added onSaving hook");
			UnityEditor.SceneManagement.EditorSceneManager.sceneSaving += (scene, _) => TrackableSceneSaveHelper.ValidateSceneTrackableIds(scene);
		}
	}
}