using EventHorizon.FileFormat;
using EventHorizon.Trackables;
using UnityEngine;

namespace EventHorizon.MetaXR
{
	public class Utils
	{
		public static T AddTrackable<T, TU>(GameObject targetGameObject, TrackableID id)
			where T : BaseTrackableComponent<TU> where TU : IPacket
		{
			var trackableComponent = targetGameObject.AddComponent<T>();
			trackableComponent.Id = id;
			TrackableManagerComponent.Instance.Register(trackableComponent);
			return trackableComponent;
		}

		public static TransformTrackableComponent AddTransformTrackable(GameObject targetGameObject, TrackableID id,
			bool isLocal = false)
		{
			var trackableComponent = AddTrackable<TransformTrackableComponent, TransformPacket>(targetGameObject, id);
			trackableComponent.isLocal = isLocal;
			return trackableComponent;
		}

		public static ActivationTrackableComponent
			AddActivationTrackable(GameObject targetGameObject, TrackableID id) =>
			AddTrackable<ActivationTrackableComponent, ActivationPacket>(targetGameObject, id);
	}
}