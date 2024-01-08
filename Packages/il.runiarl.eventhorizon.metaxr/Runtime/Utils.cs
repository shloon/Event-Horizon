using UnityEngine;

namespace EventHorizon.MetaXR
{
	public class Utils
	{
		public static TransformTrackableComponent AddTrackableToGameObject(GameObject gameObj, TrackableID id, bool isLocal = false)
		{
			var trackable = gameObj.AddComponent<TransformTrackableComponent>();
			trackable.Id = id;
			trackable.isLocal = isLocal;
			TrackableManagerComponent.Instance.Register(trackable);

			return trackable;
		}
	}
}