using UnityEngine;

namespace EventHorizon.MetaXR
{
	public class Utils
	{
		public static void AddTrackableToGameObject(GameObject gameObj, TrackableID id)
		{
			Debug.Log(gameObj);

			var trackable = gameObj.AddComponent<TransformTrackableComponent>();
			trackable.Id = id;
		}
	}
}