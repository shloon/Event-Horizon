using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace EventHorizon.MetaXR
{
	[RequireComponent(typeof(OVRRuntimeController))]
	[DisallowMultipleComponent]
	public class MetaRuntimeControllerHook : MonoBehaviour
	{
		private static readonly FieldInfo m_controllerObjectField =
			typeof(OVRRuntimeController).GetField("m_controllerObject", BindingFlags.NonPublic | BindingFlags.Instance);

		private static readonly FieldInfo m_animationNodesField =
			typeof(OVRRuntimeController).GetField("m_animationNodes", BindingFlags.NonPublic | BindingFlags.Instance);

		private static readonly FieldInfo m_gameObjField =
			typeof(OVRGLTFAnimatinonNode).GetField("m_gameObj", BindingFlags.NonPublic | BindingFlags.Instance);

		// TODO: test this on controllers other than Quest 2 Touch Controllers
		public TrackableIDWrapper parentID;
		public TrackableIDWrapper controllerID;
		public TrackableIDWrapper laserID;
		public TrackableIDWrapper controllerButton0ID;
		public TrackableIDWrapper controllerButton1ID;
		public TrackableIDWrapper controllerButton2ID;
		public TrackableIDWrapper controllerButton3ID;
		public TrackableIDWrapper controllerButton4ID;
		public TrackableIDWrapper controllerButton5ID;
		private Dictionary<OVRGLTFInputNode, OVRGLTFAnimatinonNode> animationNodes;
		private GameObject controllerGameObject;

		private bool initialized;
		private Transform parentTransform;
		private OVRRuntimeController runtimeController;

		private void Start()
		{
			parentTransform = transform.parent;
			runtimeController = GetComponent<OVRRuntimeController>();
		}

		private void Update()
		{
			if (!initialized && OVRInput.IsControllerConnected(runtimeController.m_controller) &&
			    controllerGameObject is null &&
			    animationNodes is null)
			{
				Debug.Log("Controller connected, let's find the relevant game objects");
				controllerGameObject = (GameObject) m_controllerObjectField.GetValue(runtimeController);
				animationNodes =
					(Dictionary<OVRGLTFInputNode, OVRGLTFAnimatinonNode>) m_animationNodesField.GetValue(
						runtimeController);

				if (!(controllerGameObject is null || animationNodes is null))
				{
					AddTrackers();
				}
			}
		}

		private void AddTrackers()
		{
			if (initialized)
			{
				Debug.Log("We shouldn't have called this twice.");
				return;
			}

			Utils.AddTrackableToGameObject(gameObject, controllerID.value);

			var parentGameObject = parentTransform.gameObject;
			Utils.AddTrackableToGameObject(parentGameObject, parentID.value);

			// // TODO: should we track more things?
			//
			// var childGameObjects = GetComponentsInChildren<Transform>().Select(x => x.gameObject).ToList();
			//
			// var laserGO = childGameObjects.FirstOrDefault(x => x.name.Contains("laser_begin"));
			// if (laserGO is not null)
			// {
			// 	Utils.AddTrackableToGameObject(laserGO, laserID.value);
			// }

			// these might not exist on every controller, so we check if each of them exists.
			// this assumes that we won't change to a different kind of controllers in the middle of the experiment.
			if (animationNodes.TryGetValue(OVRGLTFInputNode.Button_A_X, out var button0))
			{
				var controllerButton0GO = (GameObject) m_gameObjField.GetValue(button0);
				Utils.AddTrackableToGameObject(controllerButton0GO, controllerButton0ID.value);
			}

			if (animationNodes.TryGetValue(OVRGLTFInputNode.Button_B_Y, out var button1))
			{
				var controllerButton1GO = (GameObject) m_gameObjField.GetValue(button1);
				Utils.AddTrackableToGameObject(controllerButton1GO, controllerButton1ID.value);
			}

			if (animationNodes.TryGetValue(OVRGLTFInputNode.Button_Oculus_Menu, out var button2))
			{
				var controllerButton2GO = (GameObject) m_gameObjField.GetValue(button2);
				Utils.AddTrackableToGameObject(controllerButton2GO, controllerButton2ID.value);
			}

			if (animationNodes.TryGetValue(OVRGLTFInputNode.Trigger_Grip, out var button3))
			{
				var controllerButton3GO = (GameObject) m_gameObjField.GetValue(button3);
				Utils.AddTrackableToGameObject(controllerButton3GO, controllerButton3ID.value);
			}

			if (animationNodes.TryGetValue(OVRGLTFInputNode.Trigger_Front, out var button4))
			{
				var controllerButton4GO = (GameObject) m_gameObjField.GetValue(button4);
				Utils.AddTrackableToGameObject(controllerButton4GO, controllerButton4ID.value);
			}

			if (animationNodes.TryGetValue(OVRGLTFInputNode.ThumbStick, out var button5))
			{
				var controllerButton5GO = (GameObject) m_gameObjField.GetValue(button5);
				Utils.AddTrackableToGameObject(controllerButton5GO, controllerButton5ID.value);
			}

			initialized = true;
		}
	}
}