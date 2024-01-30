using EventHorizon.FileFormat;
using EventHorizon.Trackables;
using UnityEngine;

namespace EventHorizon.MetaXR
{
	[RequireComponent(typeof(OVRHand))]
	[RequireComponent(typeof(OVRSkeleton))] // ensures that we have access to the bones
	[DisallowMultipleComponent]
	public class MetaHandsHook : MonoBehaviour
	{
		public TrackableIDWrapper handID;
		public TrackableIDWrapper handActivationID;

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

		private bool initialized;


		private OVRSkeleton skeleton;

		public void Start() => skeleton = GetComponent<OVRSkeleton>();

		public void Update()
		{
			if (!initialized)
			{
				if (skeleton.IsInitialized)
				{
					AssignTrackablesToBones();
				}
			}
		}

		public void AssignTrackablesToBones()
		{
			void AddTrackableToBone(OVRSkeleton.BoneId boneId, TrackableIDWrapper boneID)
			{
				Utils.AddTransformTrackable(skeleton.Bones[(int) boneId].Transform.gameObject, boneID.value);
			}

			Utils.AddTransformTrackable(gameObject, handID.value);

			var geometryGO = gameObject.transform.GetChild(0).gameObject;
			if (geometryGO != null)
			{
				Utils.AddTrackable<ActivationTrackableComponent, ActivationPacket>(geometryGO, handActivationID.value);
			}

			AddTrackableToBone(OVRSkeleton.BoneId.Hand_WristRoot, wristRoot_BoneId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_ForearmStub, forearmStub_BoneId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Thumb0, thumb0_BoneId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Thumb1, thumb1_BoneId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Thumb2, thumb2_BoneId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Thumb3, thumb3_BoneId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Index1, index1_BoneId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Index2, index2_BoneId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Index3, index3_BoneId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Middle1, middle1_BoneId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Middle2, middle2_BoneId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Middle3, middle3_BoneId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Ring1, ring1_BoneId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Ring2, ring2_BoneId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Ring3, ring3_BoneId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Pinky0, pinky0_BoneId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Pinky1, pinky1_BoneId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Pinky2, pinky2_BoneId);
			AddTrackableToBone(OVRSkeleton.BoneId.Hand_Pinky3, pinky3_BoneId);

			initialized = true;
		}
	}
}