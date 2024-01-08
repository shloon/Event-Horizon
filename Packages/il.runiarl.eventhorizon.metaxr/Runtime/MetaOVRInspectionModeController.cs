﻿using UnityEngine;

namespace EventHorizon.MetaXR
{
	public class MetaOVRInspectionModeController : MonoBehaviour
	{
		public TrackableIDWrapper controllerID = new();
		public TrackableIDWrapper skeletonID = new();
		public TrackableIDWrapper controllerButton0ID = new();
		public TrackableIDWrapper controllerButton1ID = new();
		public TrackableIDWrapper controllerButton2ID = new();
		public TrackableIDWrapper controllerButton3ID = new();
		public TrackableIDWrapper controllerButton4ID = new();
		public TrackableIDWrapper controllerButton5ID = new();

		private void OnEnable()
		{
			Utils.AddTrackableToGameObject(gameObject, controllerID.value);

			foreach (var childTransform in GetComponentsInChildren<Transform>())
			{
				var childTransformGameObject = childTransform.gameObject;
				var gameObjectName = childTransformGameObject.name;
				switch (gameObjectName)
				{
					case "b_button_x":
					case "b_button_a":
						Utils.AddTrackableToGameObject(childTransformGameObject, controllerButton0ID.value, isLocal: true);
						break;
					
					case "b_button_b":
					case "b_button_y":
						Utils.AddTrackableToGameObject(childTransformGameObject, controllerButton1ID.value, isLocal: true);
						break;
					
					case "b_button_oculus":
						Utils.AddTrackableToGameObject(childTransformGameObject, controllerButton2ID.value, isLocal: true);
						break;
					
					case "b_trigger_grip":
						Utils.AddTrackableToGameObject(childTransformGameObject, controllerButton3ID.value, isLocal: true);
						break;
					
					case "b_trigger_front":
						Utils.AddTrackableToGameObject(childTransformGameObject, controllerButton4ID.value, isLocal: true);
						break;
					
					case "b_thumbstick":
						Utils.AddTrackableToGameObject(childTransformGameObject, controllerButton5ID.value, isLocal: true);
						break;
				}
				
				if(gameObjectName.Contains("controller_world"))
					Utils.AddTrackableToGameObject(childTransformGameObject, skeletonID.value, isLocal: false);
			}
		}
	}
}