using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace EventHorizon
{
	public class TransformControlAsset : PlayableAsset
	{
		public TransformData[] data;
		public RecordingMetadata metadata;

		public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
		{
			var playable = ScriptPlayable<TransformControl>.Create(graph);

			var transformControlBehaviour = playable.GetBehaviour();
			transformControlBehaviour.data = data;
			transformControlBehaviour.metadata = metadata;

			return playable;
		}
	}

	[System.Serializable]
	public class TransformControl : PlayableBehaviour
	{
		public TransformData[] data;
		public RecordingMetadata metadata;

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			// try getting the underlying transform
			Transform transform = playerData as Transform;
			transform ??= (playerData as Animator)?.gameObject.transform;

			if (transform == null)
				return;

			var playableDirector = (PlayableDirector) playable.GetGraph().GetResolver();

			var time = playableDirector.time;
			var frame = (ulong) (time * metadata.fps.GetAsDouble()); // time * fps = frames
			Debug.Log($"Time: {time}, guessed frame: {frame}, actual frame: {info.frameId}, output: {info.evaluationType}");

			transform.position = data[frame].position;
			transform.rotation = data[frame].rotation;
			transform.localScale = data[frame].scale;
		}

		public void RunOnAllBoundObjects(Playable playable, Action<GameObject> action)
		{
			var graph = playable.GetGraph();
			var director = ((PlayableDirector) graph.GetResolver());
			if (director == null) return;

			for (var i = 0; i < graph.GetOutputCount(); ++i)
			{
				var track = graph.GetOutput(i).GetReferenceObject();
				var go = ((Animator) director.GetGenericBinding(track))?.gameObject;
				if (go) action(go);
			}
		}

		private enum State
		{
			Unknown,
			BehaviourPaused,
			BehaviourPlay,
		}

		private State state = State.Unknown;

		public override void OnBehaviourPause(Playable playable, FrameData info)
		{
			if (state != State.BehaviourPaused)
				RunOnAllBoundObjects(playable, go => ToggleDangerousComponents(go, true));
			state = State.BehaviourPlay;
			base.OnBehaviourPause(playable, info);
		}

		public override void OnBehaviourPlay(Playable playable, FrameData info)
		{
			if (state != State.BehaviourPlay)
				RunOnAllBoundObjects(playable, go => ToggleDangerousComponents(go, false));
			state = State.BehaviourPlay;
			base.OnBehaviourPlay(playable, info);
		}

		private void ToggleDangerousComponents(GameObject go, bool enable)
		{
			var rigidbody = go.GetComponent<Rigidbody>();
			rigidbody.isKinematic = !enable; // equivalent to disable on a rigidbody?
		}
	}

	[TrackClipType(typeof(TransformControlAsset))]
	[TrackBindingType(typeof(Transform))]
	public class TransformControlTrack : TrackAsset
	{
	}
}