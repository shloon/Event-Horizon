using UnityEngine;

namespace EventHorizon.MetaXR
{
	[RequireComponent(typeof(OVRHand))]
	public class MetaInspectionModeHands : MonoBehaviour
	{
		[Header("Trackable IDs")]
		public TrackableIDWrapper handID;
		public TrackableIDWrapper wristRoot_BoneId;
		public TrackableIDWrapper forearmStub_BoneId;
		public TrackableIDWrapper thumb0_BoneId;
		public TrackableIDWrapper thumb1_BoneId;
		public TrackableIDWrapper thumb2_BoneId;
		public TrackableIDWrapper thumb3_BoneId;
		public TrackableIDWrapper index1_BoneId;
		public TrackableIDWrapper index2_BoneId;
		public TrackableIDWrapper index3_BoneId;
		public TrackableIDWrapper middle1_BoneId;
		public TrackableIDWrapper middle2_BoneId;
		public TrackableIDWrapper middle3_BoneId;
		public TrackableIDWrapper ring1_BoneId;
		public TrackableIDWrapper ring2_BoneId;
		public TrackableIDWrapper ring3_BoneId;
		public TrackableIDWrapper pinky0_BoneId;
		public TrackableIDWrapper pinky1_BoneId;
		public TrackableIDWrapper pinky2_BoneId;
		public TrackableIDWrapper pinky3_BoneId;

		[Header("Bones")]
		public Transform wristRootBone;
		public Transform forearmStubBone;
		public Transform thumb0Bone;
		public Transform thumb1Bone;
		public Transform thumb2Bone;
		public Transform thumb3Bone;
		public Transform index1Bone;
		public Transform index2Bone;
		public Transform index3Bone;
		public Transform middle1Bone;
		public Transform middle2Bone;
		public Transform middle3Bone;
		public Transform ring1Bone;
		public Transform ring2Bone;
		public Transform ring3Bone;
		public Transform pinky0Bone;
		public Transform pinky1Bone;
		public Transform pinky2Bone;
		public Transform pinky3Bone;

		private bool initialized;

		public void OnEnable()
		{
			if (initialized) return;

			Utils.AddTransformTrackable(gameObject, handID.value);

			Utils.AddTransformTrackable(wristRootBone.gameObject, wristRoot_BoneId.value);
			Utils.AddTransformTrackable(forearmStubBone.gameObject, forearmStub_BoneId.value);
			Utils.AddTransformTrackable(thumb0Bone.gameObject, thumb0_BoneId.value);
			Utils.AddTransformTrackable(thumb1Bone.gameObject, thumb1_BoneId.value);
			Utils.AddTransformTrackable(thumb2Bone.gameObject, thumb2_BoneId.value);
			Utils.AddTransformTrackable(thumb3Bone.gameObject, thumb3_BoneId.value);
			Utils.AddTransformTrackable(index1Bone.gameObject, index1_BoneId.value);
			Utils.AddTransformTrackable(index2Bone.gameObject, index2_BoneId.value);
			Utils.AddTransformTrackable(index3Bone.gameObject, index3_BoneId.value);
			Utils.AddTransformTrackable(middle1Bone.gameObject, middle1_BoneId.value);
			Utils.AddTransformTrackable(middle2Bone.gameObject, middle2_BoneId.value);
			Utils.AddTransformTrackable(middle3Bone.gameObject, middle3_BoneId.value);
			Utils.AddTransformTrackable(ring1Bone.gameObject, ring1_BoneId.value);
			Utils.AddTransformTrackable(ring2Bone.gameObject, ring2_BoneId.value);
			Utils.AddTransformTrackable(ring3Bone.gameObject, ring3_BoneId.value);
			Utils.AddTransformTrackable(pinky0Bone.gameObject, pinky0_BoneId.value);
			Utils.AddTransformTrackable(pinky1Bone.gameObject, pinky1_BoneId.value);
			Utils.AddTransformTrackable(pinky2Bone.gameObject, pinky2_BoneId.value);
			Utils.AddTransformTrackable(pinky3Bone.gameObject, pinky3_BoneId.value);
		}
	}
}