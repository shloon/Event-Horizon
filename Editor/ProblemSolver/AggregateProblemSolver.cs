using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EventHorizon.Editor.ProblemSolver
{
	// TODO: test
	public class AggregateProblemSolver
	{
		public AggregateProblemSolver()
		{
			ProblemSolvers = GetSceneProblemFinders();
			foreach (var solver in ProblemSolvers)
			{
				Debug.Log($"EventHorizon: Registered solver of type \"{solver.GetType()}\"");
			}
		}

		public List<ISceneProblemFinder> ProblemSolvers { get; }

		public IEnumerable<Assembly> FindAssemblies() =>
			AppDomain.CurrentDomain.GetAssemblies()
				.Where(assembly => !assembly.FullName.StartsWith("UnityEngine") &&
				                   !assembly.FullName.StartsWith("UnityEditor") &&
				                   !assembly.FullName.StartsWith("System") &&
				                   !assembly.FullName.StartsWith("Microsoft") &&
				                   !assembly.FullName.StartsWith("mscorlib"));

		public List<ISceneProblemFinder> GetSceneProblemFinders() =>
			FindAssemblies()
				.SelectMany(x => x.GetTypes()).Where(type =>
					typeof(ISceneProblemFinder).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
				.Select(Activator.CreateInstance)
				.Cast<ISceneProblemFinder>().ToList();

		public IEnumerable<IProblem> DiscoverProblemsInScene(Scene scene, ITrackableManager manager) =>
			ProblemSolvers.SelectMany(x => x.DiscoverProblemsInScene(scene, manager));

		public void DiscoverProblems(Scene scene, bool fix = true)
		{
			var trackableManager = new TrackableManager();
			var problems = DiscoverProblemsInScene(scene, trackableManager).ToList();

			foreach (var problem in problems)
			{
				if (fix)
				{
					problem.Fix();
					Debug.LogWarningFormat("EventHorizon: Fixed: {0}", problem.Description);
				}
				else { Debug.LogWarningFormat("EventHorizon: {0}", problem.Description); }
			}
		}
	}
}