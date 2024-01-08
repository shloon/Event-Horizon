using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EventHorizon.Editor.ProblemSolver
{
	public class SceneWrapper : ISceneWrapper
	{
		public Scene scene;
		public SceneWrapper(Scene scene) => this.scene = scene;
		public GameObject[] GetRootGameObjects() => scene.GetRootGameObjects();
	}

	public class GenericSceneProblemFinder : ISceneProblemFinder
	{
		public List<IProblem> DiscoverProblemsInScene(Scene scene, ITrackableManager trackableManager) =>
			DiscoverProblemsInScene(new SceneWrapper(scene), trackableManager);

		public List<IProblem> DiscoverProblemsInScene(ISceneWrapper scene, ITrackableManager trackableManager)
		{
			var allTrackables = scene.GetRootGameObjects()
				.SelectMany(rootGO => rootGO.GetComponentsInChildren<ITrackable>())
				.ToList();
			var problems = new List<IProblem>();

			foreach (var trackable in allTrackables)
			{
				// Tracker ID was not properly set
				if (!trackable.Id.IsValid)
				{
					problems.Add(new InvalidTrackableIDProblem
					{
						trackable = trackable, trackableManager = trackableManager
					});
				}
				// Tracker's ID is already being used by another
				else if (trackableManager.RegisteredTrackables.TryGetValue(trackable.Id, out var other) &&
				         !trackable.Equals(other))
				{
					problems.Add(new TwoTrackablesWithSameIDProblem
					{
						trackable = trackable, otherTrackable = other, trackableManager = trackableManager
					});
				}
				// Tracker is valid, so let's register it. We do this in one go since it's more efficient;
				else
				{
					trackableManager.Register(trackable);
				}
			}

			return problems;
		}
	}
}