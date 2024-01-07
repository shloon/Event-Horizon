using UnityEditor;
using UnityEngine;

namespace EventHorizon.MetaXR.Editor
{
	public static class MetaOneClicksHooks
	{
		private const string rootPath = "GameObject/EventHorizon/One-Click Integrations/";

		private static bool ValidateOneClick<T>() where T : Component =>
			Selection.activeGameObject != null &&
			Selection.activeGameObject.GetComponent<T>() != null;

		private static T GetSelectedComponent<T>() => Selection.activeGameObject.GetComponent<T>();

		[MenuItem(rootPath + "MetaXR Player Rig", true)]
		public static bool Validate_MetaOVRPlayer_OneClick() => ValidateOneClick<OVRPlayerController>();

		[MenuItem(rootPath + "MetaXR Player Rig")]
		public static void Execute_MetaOVRPlayer_OneClick() =>
			MetaOneClicks.DoOneClickPlayer(GetSelectedComponent<OVRPlayerController>());

		[MenuItem(rootPath + "MetaXR Camera Rig", true)]
		public static bool Validate_MetaOVRCamera_OneClick() => ValidateOneClick<OVRCameraRig>();

		[MenuItem(rootPath + "MetaXR Camera Rig")]
		public static void Execute_MetaOVRCamera_OneClick() =>
			MetaOneClicks.DoOneClickCamera(GetSelectedComponent<OVRCameraRig>());

		[MenuItem(rootPath + "MetaXR Runtime Controller", true)]
		public static bool Validate_MetaOVRRuntimeController_OneClick() => ValidateOneClick<OVRRuntimeController>();

		[MenuItem(rootPath + "MetaXR Runtime Controller")]
		public static void Execute_MetaOVRRuntimeController_OneClick() =>
			MetaOneClicks.DoOneClickController(GetSelectedComponent<OVRRuntimeController>());

		[MenuItem(rootPath + "MetaXR Hand", true)]
		public static bool Validate_MetaOVRHand_OneClick() => ValidateOneClick<OVRHand>();

		[MenuItem(rootPath + "MetaXR Hand")]
		public static void Execute_MetaOVRHand_OneClick() =>
			MetaOneClicks.DoOneClickHand(GetSelectedComponent<OVRHand>());

		[MenuItem("Event Horizon/Apply One-Click Integration", true)]
		public static bool Validate_DoAutoGuess() =>
			Validate_MetaOVRPlayer_OneClick() || Validate_MetaOVRCamera_OneClick() ||
			Validate_MetaOVRRuntimeController_OneClick() || Validate_MetaOVRHand_OneClick();

		[MenuItem("Event Horizon/Apply One-Click Integration")]
		[MenuItem(rootPath + "Auto-guess", priority = 11)]
		public static void Do_AutoGuess()
		{
			if (Selection.activeGameObject == null)
			{
				Debug.Log("EventHorizon: OneClick: No object was selected, aborting...");
			}

			var playerController = Selection.activeGameObject.GetComponent<OVRPlayerController>();
			if (playerController != null)
			{
				MetaOneClicks.DoOneClickPlayer(playerController);
				return;
			}

			var cameraRig = Selection.activeGameObject.GetComponent<OVRCameraRig>();
			if (cameraRig != null)
			{
				MetaOneClicks.DoOneClickCamera(cameraRig);
				return;
			}

			var hand = Selection.activeGameObject.GetComponent<OVRHand>();
			if (hand != null)
			{
				MetaOneClicks.DoOneClickHand(hand);
				return;
			}

			var runtimeController = Selection.activeGameObject.GetComponent<OVRRuntimeController>();
			if (runtimeController != null)
			{
				MetaOneClicks.DoOneClickController(runtimeController);
			}
		}
	}

	public static class MetaOneClicks
	{
		public static void DoOneClickPlayer(OVRPlayerController playerController, bool undo = false)
		{
			Debug.Log("EventHorizon: OneClick: Applying one-click integration on player rig");
			var playerHook = playerController.gameObject.GetComponent<MetaOVRPlayerHook>();
			if (playerHook == null)
			{
				playerHook = playerController.gameObject.AddComponent<MetaOVRPlayerHook>();
			}

			if (playerHook.GetCameraRig() != null)
			{
				DoOneClickCamera(playerHook.GetCameraRig());
			}
		}

		public static void DoOneClickCamera(OVRCameraRig cameraRig, bool undo = false)
		{
			Debug.Log("EventHorizon: OneClick: Applying one-click integration on camera rig");
			var cameraHook = cameraRig.gameObject.GetComponent<MetaOVRCameraHook>();
			if (cameraHook == null)
			{
				cameraHook = cameraRig.gameObject.AddComponent<MetaOVRCameraHook>();
			}

			// try locating hands and controlllers
			foreach (var runtimeController in cameraRig.GetComponentsInChildren<OVRRuntimeController>())
			{
				if (runtimeController.GetComponent<MetaRuntimeControllerHook>() == null)
				{
					DoOneClickController(runtimeController);
				}
			}

			foreach (var hand in cameraRig.GetComponentsInChildren<OVRHand>())
			{
				if (hand.GetComponent<MetaHandsHook>() == null)
				{
					DoOneClickHand(hand);
				}
			}
		}

		public static void DoOneClickHand(OVRHand hand, bool undo = false)
		{
			Debug.Log("EventHorizon: OneClick: Applying one-click integration on hand prefab");
			var handHook = hand.gameObject.GetComponent<MetaHandsHook>();
			if (handHook == null)
			{
				handHook = hand.gameObject.AddComponent<MetaHandsHook>();
			}
		}

		public static void DoOneClickController(OVRRuntimeController controller, bool undo = false)
		{
			Debug.Log("EventHorizon: OneClick: Applying one-click integration on runtime controller prefab");
			var controllerHook = controller.gameObject.GetComponent<MetaRuntimeControllerHook>();
			if (controllerHook == null)
			{
				controllerHook = controller.gameObject.AddComponent<MetaRuntimeControllerHook>();
			}
		}
	}
}