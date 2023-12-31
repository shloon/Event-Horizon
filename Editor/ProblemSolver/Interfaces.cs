using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EventHorizon.Editor.ProblemSolver
{
	public interface IProblem
	{
		string Description { get; }
		void Fix();
	}

	public interface ISceneProblemFinder
	{
		public List<IProblem> DiscoverProblemsInScene(Scene scene, ITrackableManager trackableManager);
	}

	public interface ISceneWrapper
	{
		public GameObject[] GetRootGameObjects();
	}
}