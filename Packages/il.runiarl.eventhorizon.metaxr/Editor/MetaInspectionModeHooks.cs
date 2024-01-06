using EventHorizon.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace EventHorizon.MetaXR.Editor
{
	[InitializeOnLoad]
	public static class MetaInspectionModeHooks
	{
		static MetaInspectionModeHooks()
		{
			InspectionModeHook.onInspectionModeStart += InspectionHook_MetaOVRPlayerHook;
			InspectionModeHook.onInspectionModeStart += InspectionHook_MetaOVRCameraHook;
			InspectionModeHook.onInspectionModeStart += InspectionHook_MetaRuntimeControllerHook;
			InspectionModeHook.onInspectionModeStart += InspectionHook_MetaHandsHook;
		}

		public static void InspectionHook_MetaOVRPlayerHook()
		{
			foreach (var ovrPlayerHook in Object.FindObjectsOfType<MetaOVRPlayerHook>())
			{
				// TODO introduce more sophisticated condition
				if (ovrPlayerHook.gameObject.scene != SceneManager.GetActiveScene())
				{
					continue;
				}

				ovrPlayerHook.GetComponent<OVRPlayerController>().enabled = false;
				ovrPlayerHook.GetComponent<CharacterController>().enabled = false;
			}
		}

		public static void InspectionHook_MetaOVRCameraHook()
		{
			foreach (var ovrCameraHook in Object.FindObjectsOfType<MetaOVRCameraHook>())
			{
				// TODO introduce more sophisticated condition
				if (ovrCameraHook.gameObject.scene != SceneManager.GetActiveScene())
				{
					continue;
				}

				ovrCameraHook.GetComponent<OVRCameraRig>().enabled = false;
				ovrCameraHook.GetComponent<OVRManager>().enabled = false;
				ovrCameraHook.GetComponent<OVRCameraRig>().enabled = false;
			}
		}

		public static void InspectionHook_MetaRuntimeControllerHook()
		{
			foreach (var runtimeControllerHook in Object.FindObjectsOfType<MetaRuntimeControllerHook>())
			{
				// TODO introduce more sophisticated condition
				if (runtimeControllerHook.gameObject.scene != SceneManager.GetActiveScene())
				{
				}

				// TODO replace with runtime controller model based on data from recording
			}
		}

		public static void InspectionHook_MetaHandsHook()
		{
			foreach (var handsHook in Object.FindObjectsOfType<MetaHandsHook>())
			{
				// TODO introduce more sophisticated condition
				if (handsHook.gameObject.scene != SceneManager.GetActiveScene())
				{
				}

				// TODO replace with play-time controller model based on data from recording
			}
		}
	}
}