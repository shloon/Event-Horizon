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

		public void Start()
		{
			rig = GetComponent<OVRCameraRig>();

			Utils.AddTrackableToGameObject(gameObject, cameraID.value);

			Utils.AddTrackableToGameObject(rig.trackingSpace.gameObject, trackingSpaceID.value);
			Utils.AddTrackableToGameObject(rig.leftEyeAnchor.gameObject, leftEyeAnchorID.value);
			Utils.AddTrackableToGameObject(rig.centerEyeAnchor.gameObject, centerEyeAnchorID.value);
			Utils.AddTrackableToGameObject(rig.rightEyeAnchor.gameObject, rightEyeAnchorID.value);
			Utils.AddTrackableToGameObject(rig.leftHandAnchor.gameObject, leftHandAnchorID.value);
			Utils.AddTrackableToGameObject(rig.rightHandAnchor.gameObject, rightHandAnchorID.value);
			Utils.AddTrackableToGameObject(rig.leftHandAnchorDetached.gameObject, leftHandAnchorDetachedID.value);
			Utils.AddTrackableToGameObject(rig.rightHandAnchorDetached.gameObject, rightHandAnchorDetachedID.value);
			Utils.AddTrackableToGameObject(rig.leftControllerInHandAnchor.gameObject,
				leftControllerInHandAnchorID.value);
			Utils.AddTrackableToGameObject(rig.leftHandOnControllerAnchor.gameObject,
				leftHandOnControllerAnchorID.value);
			Utils.AddTrackableToGameObject(rig.rightControllerInHandAnchor.gameObject,
				rightControllerInHandAnchorID.value);
			Utils.AddTrackableToGameObject(rig.rightHandOnControllerAnchor.gameObject,
				rightHandOnControllerAnchorID.value);
			Utils.AddTrackableToGameObject(rig.leftControllerAnchor.gameObject, leftControllerAnchorID.value);
			Utils.AddTrackableToGameObject(rig.rightControllerAnchor.gameObject, rightControllerAnchorID.value);
			Utils.AddTrackableToGameObject(rig.trackerAnchor.gameObject, trackerAnchorID.value);
		}
	}
}