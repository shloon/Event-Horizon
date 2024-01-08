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
			if (!Enum.TryParse<OVRPlugin.SystemHeadset>(InspectionModeHook.FormatData.vrMetadataPacket.headsetType,
				    out var currentHeadset))
			{
				currentHeadset = OVRPlugin.SystemHeadset.None;
			}

			if (!Enum.TryParse<OVRPlugin.InteractionProfile>(
				    InspectionModeHook.FormatData.vrMetadataPacket.interactionProfile, out var interactionProfile))
			{
				interactionProfile = OVRPlugin.InteractionProfile.None;
			}

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
				var inspectionModeController =
					runtimeControllerHook.gameObject.AddComponent<MetaOVRInspectionModeController>();
				inspectionModeController.controllerID = runtimeControllerHook.controllerID;
				inspectionModeController.controllerActivationID = runtimeControllerHook.controllerActivationID;
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
				"Packages/il.runiarl.eventhorizon.metaxr/Prefabs/MetaHand_L.prefab");
			var rightHandPrefab =
				AssetDatabase.LoadAssetAtPath<GameObject>(
					"Packages/il.runiarl.eventhorizon.metaxr/Prefabs/MetaHand_R.prefab");

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

				// disable old hands hook to un-register the associated IDs
				handsHook.enabled = false;

				// enable hand prefab. no need to disable the game object since the component is initially disabled
				var inspectionModeHand = newGO.GetComponent<MetaInspectionModeHands>();
				inspectionModeHand.handID = handsHook.handID;
				inspectionModeHand.wristRoot_BoneId = handsHook.wristRoot_BoneId;
				inspectionModeHand.forearmStub_BoneId = handsHook.forearmStub_BoneId;
				inspectionModeHand.thumb0_BoneId = handsHook.thumb0_BoneId;
				inspectionModeHand.thumb1_BoneId = handsHook.thumb1_BoneId;
				inspectionModeHand.thumb2_BoneId = handsHook.thumb2_BoneId;
				inspectionModeHand.thumb3_BoneId = handsHook.thumb3_BoneId;
				inspectionModeHand.index1_BoneId = handsHook.index1_BoneId;
				inspectionModeHand.index2_BoneId = handsHook.index2_BoneId;
				inspectionModeHand.index3_BoneId = handsHook.index3_BoneId;
				inspectionModeHand.middle1_BoneId = handsHook.middle1_BoneId;
				inspectionModeHand.middle2_BoneId = handsHook.middle2_BoneId;
				inspectionModeHand.middle3_BoneId = handsHook.middle3_BoneId;
				inspectionModeHand.ring1_BoneId = handsHook.ring1_BoneId;
				inspectionModeHand.ring2_BoneId = handsHook.ring2_BoneId;
				inspectionModeHand.ring3_BoneId = handsHook.ring3_BoneId;
				inspectionModeHand.pinky0_BoneId = handsHook.pinky0_BoneId;
				inspectionModeHand.pinky1_BoneId = handsHook.pinky1_BoneId;
				inspectionModeHand.pinky2_BoneId = handsHook.pinky2_BoneId;
				inspectionModeHand.pinky3_BoneId = handsHook.pinky3_BoneId;
				inspectionModeHand.enabled = true;

				// delete old hand prefab
				Object.DestroyImmediate(handsHook.gameObject);
			}
		}
	}
}