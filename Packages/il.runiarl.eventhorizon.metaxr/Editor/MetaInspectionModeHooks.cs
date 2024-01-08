using EventHorizon.Editor;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace EventHorizon.MetaXR.Editor
{
	[InitializeOnLoad]
	public static class MetaInspectionModeHooks
	{
		public enum ControllerType { Rift, QuestAndRiftS, Quest2, TouchPro, TouchPlus }

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


		public static ControllerType GetControllerType(OVRPlugin.SystemHeadset headset,
			OVRPlugin.InteractionProfile profile)
		{
			switch (headset)
			{
				case OVRPlugin.SystemHeadset.Rift_CV1:
					return ControllerType.Rift;

				case OVRPlugin.SystemHeadset.Oculus_Quest_2:
				case OVRPlugin.SystemHeadset.Oculus_Link_Quest_2:
					return profile == OVRPlugin.InteractionProfile.TouchPro
						? ControllerType.TouchPro
						: ControllerType.Quest2;

				case OVRPlugin.SystemHeadset.Meta_Quest_Pro:
					return ControllerType.TouchPro;

				case OVRPlugin.SystemHeadset.Meta_Link_Quest_Pro:
					return ControllerType.TouchPro;

				case OVRPlugin.SystemHeadset.Meta_Quest_3:
				case OVRPlugin.SystemHeadset.Meta_Link_Quest_3:
					return profile == OVRPlugin.InteractionProfile.TouchPro
						? ControllerType.TouchPro
						: ControllerType.TouchPlus;

				default:
					return ControllerType.QuestAndRiftS;
			}
		}

		public static void InspectionHook_MetaRuntimeControllerHook()
		{
			// TODO actually get this from somewhere
			var currentHeadset = OVRPlugin.SystemHeadset.Meta_Link_Quest_Pro;
			var interactionProfile = OVRPlugin.InteractionProfile.TouchPro;
			var controllerType = GetControllerType(currentHeadset, interactionProfile);
			var (leftController, rightController) = GetControllerMeshes(controllerType);

			foreach (var runtimeControllerHook in Object.FindObjectsOfType<MetaRuntimeControllerHook>())
			{
				// TODO introduce more sophisticated condition
				if (runtimeControllerHook.gameObject.scene != SceneManager.GetActiveScene())
				{
				}

				var runtimeController = runtimeControllerHook.GetComponent<OVRRuntimeController>();
				runtimeController.enabled = false;

				var isRightHand = (runtimeController.m_controller & OVRInput.Controller.RTouch) != 0;
				var isLeftHand = (runtimeController.m_controller & OVRInput.Controller.LTouch) != 0;

				GameObject newGO = null;
				var controllerPrefab = isRightHand ? rightController : isLeftHand ? leftController : null;
				if (controllerPrefab)
				{
					newGO = Object.Instantiate(controllerPrefab, runtimeControllerHook.transform);
				}

				if (newGO == null)
				{
					continue;
				}
				
				runtimeControllerHook.gameObject.SetActive(false);
				var inspectionModeController = runtimeControllerHook.gameObject.AddComponent<MetaOVRInspectionModeController>();
				inspectionModeController.controllerID = runtimeControllerHook.controllerID;
				inspectionModeController.skeletonID = runtimeControllerHook.skeletonID;
				inspectionModeController.controllerButton0ID = runtimeControllerHook.controllerButton0ID;
				inspectionModeController.controllerButton1ID = runtimeControllerHook.controllerButton1ID;
				inspectionModeController.controllerButton2ID = runtimeControllerHook.controllerButton2ID;
				inspectionModeController.controllerButton3ID = runtimeControllerHook.controllerButton3ID;
				inspectionModeController.controllerButton4ID = runtimeControllerHook.controllerButton4ID;
				inspectionModeController.controllerButton5ID = runtimeControllerHook.controllerButton5ID;
				runtimeControllerHook.gameObject.SetActive(true);

				Object.DestroyImmediate(runtimeControllerHook);
				Object.DestroyImmediate(runtimeController);
			}
		}

		private static (GameObject leftController, GameObject rightController) GetControllerMeshes(
			ControllerType controllerType)
		{
			GameObject leftController;
			GameObject rightController;
			switch (controllerType)
			{
				case ControllerType.Rift:
					leftController = AssetDatabase.LoadAssetAtPath<GameObject>(
						"Packages/com.meta.xr.sdk.core/Meshes/OculusTouchForRift/left_touch_controller_model_skel.fbx");
					rightController = AssetDatabase.LoadAssetAtPath<GameObject>(
						"Packages/com.meta.xr.sdk.core/Meshes/OculusTouchForRift/right_touch_controller_model_skel.fbx");
					break;
				case ControllerType.QuestAndRiftS:
					leftController = AssetDatabase.LoadAssetAtPath<GameObject>(
						"Packages/com.meta.xr.sdk.core/Meshes/OculusTouchForQuestAndRiftS/OculusTouchForQuestAndRiftS_Left.fbx");
					rightController = AssetDatabase.LoadAssetAtPath<GameObject>(
						"Packages/com.meta.xr.sdk.core/Meshes/OculusTouchForQuestAndRiftS/OculusTouchForQuestAndRiftS_Left.fbx");
					break;
				case ControllerType.Quest2:
					leftController = AssetDatabase.LoadAssetAtPath<GameObject>(
						"Packages/com.meta.xr.sdk.core/Meshes/OculusTouchForQuest2/OculusTouchForQuest2_Left.fbx");
					rightController = AssetDatabase.LoadAssetAtPath<GameObject>(
						"Packages/com.meta.xr.sdk.core/Meshes/OculusTouchForQuest2/OculusTouchForQuest2_Right.fbx");
					break;
				case ControllerType.TouchPro:
					leftController = AssetDatabase.LoadAssetAtPath<GameObject>(
						"Packages/com.meta.xr.sdk.core/Meshes/MetaQuestTouchPro/MetaQuestTouchPro_Left.fbx");
					rightController = AssetDatabase.LoadAssetAtPath<GameObject>(
						"Packages/com.meta.xr.sdk.core/Meshes/MetaQuestTouchPro/MetaQuestTouchPro_Right.fbx");
					break;
				case ControllerType.TouchPlus:
					leftController = AssetDatabase.LoadAssetAtPath<GameObject>(
						"Packages/com.meta.xr.sdk.core/Meshes/MetaQuestTouchPlus/MetaQuestTouchPlus_Left.fbx");
					rightController = AssetDatabase.LoadAssetAtPath<GameObject>(
						"Packages/com.meta.xr.sdk.core/Meshes/MetaQuestTouchPlus/MetaQuestTouchPlus_Right.fbx");
					break;
				default:
					throw new Exception("This should never ever happen");
			}

			return (leftController, rightController);
		}

		public static void InspectionHook_MetaHandsHook()
		{
			var leftHandPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
				"Packages/com.meta.xr.sdk.core/Prefabs/OVRCustomHandPrefab_R.prefab");
			var rightHandPrefab =
				AssetDatabase.LoadAssetAtPath<GameObject>(
					"Packages/com.meta.xr.sdk.core/Prefabs/OVRCustomHandPrefab_R.prefab");

			foreach (var handsHook in Object.FindObjectsOfType<MetaHandsHook>())
			{
				// TODO introduce more sophisticated condition
				if (handsHook.gameObject.scene != SceneManager.GetActiveScene())
				{
				}

				// instantiate correct hand prefab
				var skeletonType = handsHook.GetComponent<OVRSkeleton>().GetSkeletonType();
				var handParentTransform = handsHook.gameObject.transform.parent;
				var newGO = skeletonType switch
				{
					OVRSkeleton.SkeletonType.HandLeft => Object.Instantiate(leftHandPrefab,
						handParentTransform),
					OVRSkeleton.SkeletonType.HandRight => Object.Instantiate(rightHandPrefab,
						handParentTransform),
					_ => null
				};

				if (newGO == null)
				{
					continue;
				}

				// enable hand prefab
				newGO.SetActive(false);
				UnityComponentHelpers.CopyComponent(handsHook, newGO);
				newGO.SetActive(true);
				Object.DestroyImmediate(handsHook.gameObject);
			}
		}
	}
}