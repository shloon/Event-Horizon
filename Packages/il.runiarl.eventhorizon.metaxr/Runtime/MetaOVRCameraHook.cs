using EventHorizon.FormatV2;
using UnityEngine;

namespace EventHorizon.MetaXR
{
	[RequireComponent(typeof(OVRCameraRig))]
	[DisallowMultipleComponent]
	public class MetaOVRCameraHook : MonoBehaviour
	{
		public TrackableIDWrapper cameraID;

		public TrackableIDWrapper trackingSpaceID;
		public TrackableIDWrapper leftEyeAnchorID;
		public TrackableIDWrapper centerEyeAnchorID;
		public TrackableIDWrapper rightEyeAnchorID;
		public TrackableIDWrapper leftHandAnchorID;
		public TrackableIDWrapper rightHandAnchorID;
		public TrackableIDWrapper leftHandAnchorDetachedID;
		public TrackableIDWrapper rightHandAnchorDetachedID;
		public TrackableIDWrapper leftControllerInHandAnchorID;
		public TrackableIDWrapper leftHandOnControllerAnchorID;
		public TrackableIDWrapper rightControllerInHandAnchorID;
		public TrackableIDWrapper rightHandOnControllerAnchorID;
		public TrackableIDWrapper leftControllerAnchorID;
		public TrackableIDWrapper rightControllerAnchorID;
		public TrackableIDWrapper trackerAnchorID;

		private OVRCameraRig rig;

		public void Awake()
		{
			rig = GetComponent<OVRCameraRig>();
		}

		public void Start()
		{
			Utils.AddTrackableToGameObject(gameObject, cameraID.value);

			Utils.AddTrackableToGameObject(rig.trackingSpace.gameObject, trackingSpaceID.value, true);
			Utils.AddTrackableToGameObject(rig.leftEyeAnchor.gameObject, leftEyeAnchorID.value, true);
			Utils.AddTrackableToGameObject(rig.centerEyeAnchor.gameObject, centerEyeAnchorID.value, true);
			Utils.AddTrackableToGameObject(rig.rightEyeAnchor.gameObject, rightEyeAnchorID.value, true);
			Utils.AddTrackableToGameObject(rig.leftHandAnchor.gameObject, leftHandAnchorID.value, true);
			Utils.AddTrackableToGameObject(rig.rightHandAnchor.gameObject, rightHandAnchorID.value, true);
			Utils.AddTrackableToGameObject(rig.leftHandAnchorDetached.gameObject, leftHandAnchorDetachedID.value, true);
			Utils.AddTrackableToGameObject(rig.rightHandAnchorDetached.gameObject, rightHandAnchorDetachedID.value,
				true);
			Utils.AddTrackableToGameObject(rig.leftControllerInHandAnchor.gameObject,
				leftControllerInHandAnchorID.value, true);
			Utils.AddTrackableToGameObject(rig.leftHandOnControllerAnchor.gameObject,
				leftHandOnControllerAnchorID.value, true);
			Utils.AddTrackableToGameObject(rig.rightControllerInHandAnchor.gameObject,
				rightControllerInHandAnchorID.value, true);
			Utils.AddTrackableToGameObject(rig.rightHandOnControllerAnchor.gameObject,
				rightHandOnControllerAnchorID.value, true);
			Utils.AddTrackableToGameObject(rig.leftControllerAnchor.gameObject, leftControllerAnchorID.value, true);
			Utils.AddTrackableToGameObject(rig.rightControllerAnchor.gameObject, rightControllerAnchorID.value, true);
			Utils.AddTrackableToGameObject(rig.trackerAnchor.gameObject, trackerAnchorID.value, true);
			
			// find recorder
			var recorder = FindObjectOfType<RecorderComponent>();
			if (recorder != null && recorder.isActiveAndEnabled)
			{
				recorder.WriteCustomPacket(new VRMetadataPacket
				{
					headsetType = OVRManager.systemHeadsetType.ToString(),
					interactionProfile = OVRPlugin.GetCurrentInteractionProfile(OVRPlugin.Hand.HandLeft).ToString()
				});
			}
		}
	}
}