using EventHorizon.FormatV2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

namespace EventHorizon
{
	public class GenericDataControlAsset : PlayableAsset
	{
		[FormerlySerializedAs("frames")] public List<GenericDataPacket> packets = new();
		public double frameDuration;

		public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
		{
			var playable = ScriptPlayable<GenericDataControlBehaviour>.Create(graph);

			var genericDataControlBehaviour = playable.GetBehaviour();
			genericDataControlBehaviour.packets = packets;
			genericDataControlBehaviour.frameDuration = frameDuration;

			return playable;
		}
	}

	[Serializable]
	public class GenericDataControlBehaviour : PlayableBehaviour
	{
		public double frameDuration;
		public List<GenericDataPacket> packets = new();

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			// try getting the underlying transform
			var transform = playerData as Transform;
			transform ??= (playerData as Animator)?.gameObject.transform;

			if (transform == null)
			{
				return;
			}

			var playableDirector = (PlayableDirector) playable.GetGraph().GetResolver();
			var frame = (int) (playableDirector.time * frameDuration);

			// TODO: do something more meaningful with this information
			Debug.Log(packets[frame].data);
		}
	}

	[TrackClipType(typeof(GenericDataControlAsset))]
	[TrackBindingType(typeof(Transform))]
	public class GenericDataControlTrack : TrackAsset
	{
	}
}