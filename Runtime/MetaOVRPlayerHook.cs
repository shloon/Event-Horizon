using UnityEngine;

namespace EventHorizon.MetaXR
{
	[RequireComponent(typeof(OVRPlayerController))]
	[DisallowMultipleComponent]
	public class MetaOVRPlayerHook : MonoBehaviour
	{
		public TrackableIDWrapper trackableID;

		private OVRCameraRig cameraRig;

		public void Start()
		{
			Utils.AddTrackableToGameObject(gameObject, trackableID.value);
			cameraRig = GetCameraRig();
		}

		public OVRCameraRig GetCameraRig()
		{
			if (cameraRig != null)
			{
				return cameraRig;
			}

			var cameraRigs = gameObject.GetComponentsInChildren<OVRCameraRig>();
			switch (cameraRigs.Length)
			{
				case 0:
					Debug.LogWarning("No OVRCameraRig attached.");
					return null;
				case 1:
					cameraRig = cameraRigs[0];
					return cameraRig;
				default:
					Debug.LogWarning("More then 1 OVRCameraRig attached.");
					return null;
			}
		}
	}
}