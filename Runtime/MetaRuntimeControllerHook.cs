using System.Collections.Generic;
using System.Linq;
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
		public TrackableIDWrapper controllerID;
		public TrackableIDWrapper controllerActivationID;
		public TrackableIDWrapper skeletonID;
		public TrackableIDWrapper controllerButton0ID;
		public TrackableIDWrapper controllerButton1ID;
		public TrackableIDWrapper controllerButton2ID;
		public TrackableIDWrapper controllerButton3ID;
		public TrackableIDWrapper controllerButton4ID;
		public TrackableIDWrapper controllerButton5ID;
		private Dictionary<OVRGLTFInputNode, OVRGLTFAnimatinonNode> animationNodes;
		private GameObject controllerGameObject;

		private bool initialized;
		private OVRRuntimeController runtimeController;

		private void Start() => runtimeController = GetComponent<OVRRuntimeController>();

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

			Utils.AddTransformTrackable(gameObject, controllerID.value);
			Utils.AddActivationTrackable(controllerGameObject, controllerActivationID.value);

			var skeletonGO = GetComponentsInChildren<Transform>().FirstOrDefault(x => x.name.Contains("skeleton"));
			if (skeletonGO != null)
			{
				Utils.AddTransformTrackable(skeletonGO.gameObject, skeletonID.value);
			}

			// these might not exist on every controller, so we check if each of them exists.
			// this assumes that we won't change to a different kind of controllers in the middle of the experiment.
			if (animationNodes.TryGetValue(OVRGLTFInputNode.Button_A_X, out var button0))
			{
				var controllerButton0GO = (GameObject) m_gameObjField.GetValue(button0);
				var trackable = Utils.AddTransformTrackable(controllerButton0GO, controllerButton0ID.value, true);
				trackable.translationMultiply = new Vector3(1, -1, -1);
				trackable.rotationMultiply = new Vector3(1, -1, -1);
			}

			if (animationNodes.TryGetValue(OVRGLTFInputNode.Button_B_Y, out var button1))
			{
				var controllerButton1GO = (GameObject) m_gameObjField.GetValue(button1);
				var trackable = Utils.AddTransformTrackable(controllerButton1GO, controllerButton1ID.value, true);
				trackable.translationMultiply = new Vector3(1, -1, -1);
				trackable.rotationMultiply = new Vector3(1, -1, -1);
			}

			if (animationNodes.TryGetValue(OVRGLTFInputNode.Button_Oculus_Menu, out var button2))
			{
				var controllerButton2GO = (GameObject) m_gameObjField.GetValue(button2);
				var trackable = Utils.AddTransformTrackable(controllerButton2GO, controllerButton2ID.value, true);
				trackable.translationMultiply = new Vector3(1, -1, -1);
				trackable.rotationMultiply = new Vector3(1, -1, -1);
			}

			if (animationNodes.TryGetValue(OVRGLTFInputNode.Trigger_Grip, out var button3))
			{
				var controllerButton3GO = (GameObject) m_gameObjField.GetValue(button3);
				var trackable = Utils.AddTransformTrackable(controllerButton3GO, controllerButton3ID.value, true);
				trackable.translationMultiply = new Vector3(1, -1, -1);
				trackable.rotationMultiply = new Vector3(1, -1, -1);
			}

			if (animationNodes.TryGetValue(OVRGLTFInputNode.Trigger_Front, out var button4))
			{
				var controllerButton4GO = (GameObject) m_gameObjField.GetValue(button4);
				var trackable = Utils.AddTransformTrackable(controllerButton4GO, controllerButton4ID.value, true);
				trackable.translationMultiply = new Vector3(1, -1, -1);
				trackable.rotationMultiply = new Vector3(1, -1, -1);
			}

			if (animationNodes.TryGetValue(OVRGLTFInputNode.ThumbStick, out var button5))
			{
				var controllerButton5GO = (GameObject) m_gameObjField.GetValue(button5);
				var trackable = Utils.AddTransformTrackable(controllerButton5GO, controllerButton5ID.value, true);
				trackable.translationMultiply = new Vector3(1, -1, -1);
				trackable.rotationMultiply = new Vector3(1, -1, -1);
			}

			initialized = true;
		}
	}
}