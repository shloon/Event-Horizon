using UnityEngine;

namespace EventHorizon.MetaXR
{
	[RequireComponent(typeof(OVRSkeleton))] // ensures that we have access to the bones
	[DisallowMultipleComponent]
	public class MetaHandsHook : MonoBehaviour
	{
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

		public TrackableIDWrapper wristRoot_BindPoseId;
		public TrackableIDWrapper forearmStub_BindPoseId;
		public TrackableIDWrapper thumb0_BindPoseId;
		public TrackableIDWrapper thumb1_BindPoseId;
		public TrackableIDWrapper thumb2_BindPoseId;
		public TrackableIDWrapper thumb3_BindPoseId;
		public TrackableIDWrapper index1_BindPoseId;
		public TrackableIDWrapper index2_BindPoseId;
		public TrackableIDWrapper index3_BindPoseId;
		public TrackableIDWrapper middle1_BindPoseId;
		public TrackableIDWrapper middle2_BindPoseId;
		public TrackableIDWrapper middle3_BindPoseId;
		public TrackableIDWrapper ring1_BindPoseId;
		public TrackableIDWrapper ring2_BindPoseId;
		public TrackableIDWrapper ring3_BindPoseId;
		public TrackableIDWrapper pinky0_BindPoseId;
		public TrackableIDWrapper pinky1_BindPoseId;
		public TrackableIDWrapper pinky2_BindPoseId;
		public TrackableIDWrapper pinky3_BindPoseId;
		private bool initialized;


		private OVRSkeleton skeleton;

		public void Start() => skeleton = GetComponent<OVRSkeleton>();

		public void Update()
		{
			if (!initialized && skeleton.IsInitialized)
			{
				AssignTrackablesToBones();
			}
		}

		public void AssignTrackablesToBones()
		{
			void AddTrackableToBone(OVRSkeleton.BoneId boneId, TrackableIDWrapper boneID, TrackableIDWrapper bindPoseID)
			{
				Utils.AddTrackableToGameObject(skeleton.Bones[(int)boneId].Transform.gameObject, boneID.value);
				Utils.AddTrackableToGameObject(skeleton.BindPoses[(int)boneId].Transform.gameObject, bindPoseID.value);
			}

			Utils.AddTrackableToGameObject(gameObject, handID.value);

			// assumes bones are ordered
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_WristRoot, wristRoot_BoneId, wristRoot_BindPoseId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_ForearmStub, forearmStub_BoneId, forearmStub_BindPoseId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Thumb0, thumb0_BoneId, thumb0_BindPoseId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Thumb1, thumb1_BoneId, thumb1_BindPoseId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Thumb2, thumb2_BoneId, thumb2_BindPoseId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Thumb3, thumb3_BoneId, thumb3_BindPoseId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Index1, index1_BoneId, index1_BindPoseId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Index2, index2_BoneId, index2_BindPoseId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Index3, index3_BoneId, index3_BindPoseId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Middle1, middle1_BoneId, middle1_BindPoseId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Middle2, middle2_BoneId, middle2_BindPoseId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Middle3, middle3_BoneId, middle3_BindPoseId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Ring1, ring1_BoneId, ring1_BindPoseId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Ring2, ring2_BoneId, ring2_BindPoseId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Ring3, ring3_BoneId, ring3_BindPoseId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Pinky0, pinky0_BoneId, pinky0_BindPoseId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Pinky1, pinky1_BoneId, pinky1_BindPoseId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Pinky2, pinky2_BoneId, pinky2_BindPoseId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Pinky3, pinky3_BoneId, pinky3_BindPoseId);

			initialized = true;
		}
	}
}