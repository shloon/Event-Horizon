using EventHorizon.Editor.ProblemSolver;
using System.Reflection;
using UnityEngine;

namespace EventHorizon.MetaXR.Editor
{
	public class TemporaryTrackable : ITrackable
	{
		public TemporaryTrackable(TrackableID id = new(), string name = "Trackable")
		{
			Name = name;
			Id = id;
		}

		public TrackableID Id { get; set; }
		public string Name { get; }
	}

	public struct NullBoneIDProblem : IProblem
	{
		public GameObject gameObject;
		public FieldInfo fieldInfo;
		public string boneName; // Name of the bone or part
		public ITrackableManager trackableManager;

		public string Description =>
			$"Trackable ID for bone \"{boneName}\" inside \"{gameObject.name}\" was null.";

		public void Fix()
		{
			var newId = trackableManager.GenerateId();
			fieldInfo.SetValue(gameObject, trackableManager.GenerateId());
			trackableManager.Register(new TemporaryTrackable(newId, $"{gameObject.name} ({boneName})"));
		}
	}

	public struct InvalidBoneTrackableIDProblem : IProblem
	{
		public GameObject gameObject;
		public TrackableIDWrapper trackableID;
		public string boneName; // Name of the bone or part
		public ITrackableManager trackableManager;

		public string Description =>
			$"Trackable ID for bone \"{boneName}\" inside \"{gameObject.name}\" is invalid.";

		public void Fix()
		{
			trackableID.value = trackableManager.GenerateId();
			trackableManager.Register(new TemporaryTrackable(trackableID.value, $"{gameObject.name} ({boneName})"));
		}
	}

	public struct BoneTrackableIDInUseProblem : IProblem
	{
		public GameObject gameObject;
		public TrackableIDWrapper trackableID;
		public string boneName; // Name of the bone or part
		public ITrackableManager trackableManager;

		public string Description =>
			$"Trackable ID for bone \"{boneName}\" inside \"{gameObject.name}\" is already in use.";

		public void Fix()
		{
			trackableID.value = trackableManager.GenerateId();
			trackableManager.Register(new TemporaryTrackable(trackableID.value, $"{gameObject.name} ({boneName})"));
		}
	}
}