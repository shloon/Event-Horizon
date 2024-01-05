using EventHorizon.FormatV2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

namespace EventHorizon
{
	public class TransformControlAsset : PlayableAsset
	{
		[FormerlySerializedAs("frames")] public List<TransformPacket> packets = new();
		public double frameDuration;

		public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
		{
			var playable = ScriptPlayable<TransformControlBehaviour>.Create(graph);

			var transformControlBehaviour = playable.GetBehaviour();
			transformControlBehaviour.packets = packets;
			transformControlBehaviour.frameDuration = frameDuration;

			return playable;
		}
	}

	[Serializable]
	public class TransformControlBehaviour : PlayableBehaviour
	{
		public List<TransformPacket> packets = new();
		public double globalStartTime;
		public double frameDuration;

		public override void OnBehaviourPlay(Playable playable, FrameData info)
		{
			var playableDirector = (PlayableDirector) playable.GetGraph().GetResolver();
			globalStartTime = playableDirector.time - playable.GetTime(); // Calculate global start time
		}

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
			var frame = (int) ((playableDirector.time - globalStartTime) / frameDuration);

			if (frame < 0 || frame >= packets.Count)
			{
				return;
			}

			transform.position = packets[frame].translation;
			transform.rotation = packets[frame].rotation;
			transform.localScale = packets[frame].scale;
		}
	}

	[TrackClipType(typeof(TransformControlAsset))]
	[TrackBindingType(typeof(Transform))]
	public class TransformControlTrack : TrackAsset
	{
	}
}