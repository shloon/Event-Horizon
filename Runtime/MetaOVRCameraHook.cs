using EventHorizon.FileFormat;
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

		public void Awake() => rig = GetComponent<OVRCameraRig>();

		public void Start()
		{
			Utils.AddTransformTrackable(gameObject, cameraID.value);

			Utils.AddTransformTrackable(rig.trackingSpace.gameObject, trackingSpaceID.value, true);
			Utils.AddTransformTrackable(rig.leftEyeAnchor.gameObject, leftEyeAnchorID.value, true);
			Utils.AddTransformTrackable(rig.centerEyeAnchor.gameObject, centerEyeAnchorID.value, true);
			Utils.AddTransformTrackable(rig.rightEyeAnchor.gameObject, rightEyeAnchorID.value, true);
			Utils.AddTransformTrackable(rig.leftHandAnchor.gameObject, leftHandAnchorID.value, true);
			Utils.AddTransformTrackable(rig.rightHandAnchor.gameObject, rightHandAnchorID.value, true);
			Utils.AddTransformTrackable(rig.leftHandAnchorDetached.gameObject, leftHandAnchorDetachedID.value, true);
			Utils.AddTransformTrackable(rig.rightHandAnchorDetached.gameObject, rightHandAnchorDetachedID.value,
				true);
			Utils.AddTransformTrackable(rig.leftControllerInHandAnchor.gameObject,
				leftControllerInHandAnchorID.value, true);
			Utils.AddTransformTrackable(rig.leftHandOnControllerAnchor.gameObject,
				leftHandOnControllerAnchorID.value, true);
			Utils.AddTransformTrackable(rig.rightControllerInHandAnchor.gameObject,
				rightControllerInHandAnchorID.value, true);
			Utils.AddTransformTrackable(rig.rightHandOnControllerAnchor.gameObject,
				rightHandOnControllerAnchorID.value, true);
			Utils.AddTransformTrackable(rig.leftControllerAnchor.gameObject, leftControllerAnchorID.value, true);
			Utils.AddTransformTrackable(rig.rightControllerAnchor.gameObject, rightControllerAnchorID.value, true);
			Utils.AddTransformTrackable(rig.trackerAnchor.gameObject, trackerAnchorID.value, true);

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