using UnityEngine;

namespace EventHorizon.MetaXR
{
	public class Utils
	{
		public static void AddTrackableToGameObject(GameObject gameObj, TrackableID id)
		{
			gameObj.SetActive(false);
			var trackable = gameObj.AddComponent<TransformTrackableComponent>();
			trackable.Id = id;
			gameObj.SetActive(true);
		}
	}
}