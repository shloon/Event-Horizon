using EventHorizon.Editor.ProblemSolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.SceneManagement;

namespace EventHorizon.MetaXR.Editor
{
	public abstract class FieldBaseIDSaveProblemsFinder : ISceneProblemFinder
	{
		private readonly Type baseType;
		private readonly IReadOnlyList<FieldInfo> idFields;

		protected FieldBaseIDSaveProblemsFinder(Type baseType)
		{
			this.baseType = baseType;
			idFields = baseType
				.GetFields(BindingFlags.Public | BindingFlags.Instance)
				.Where(field => field.FieldType == typeof(TrackableIDWrapper)).ToList();
		}

		public List<IProblem> DiscoverProblemsInScene(Scene scene,
			ITrackableManager trackableManager)
		{
			var problems = new List<IProblem>();

			var allComponents = scene.GetRootGameObjects()
				.SelectMany(rootGO => rootGO.GetComponentsInChildren(baseType))
				.ToList();

			foreach (var component in allComponents)
			{
				foreach (var fieldInfo in idFields)
				{
					var trackableID = (TrackableIDWrapper) fieldInfo.GetValue(component);
					var fieldName = fieldInfo.Name;

					if (trackableID is null)
					{
						problems.Add(new NullPartIDProblem
						{
							component = component,
							gameObject = component.gameObject,
							fieldInfo = fieldInfo,
							boneName = fieldName,
							trackableManager = trackableManager
						});
					}
					else if (!trackableID.value.IsValid)
					{
						problems.Add(new InvalidPartTrackableIDProblem
						{
							component = component,
							gameObject = component.gameObject,
							trackableID = trackableID,
							boneName = fieldName,
							trackableManager = trackableManager
						});
					}
					// if it contains the key, it's in use by someone else
					else if (trackableManager.RegisteredTrackables.ContainsKey(trackableID.value))
					{
						problems.Add(new BoneTrackableIDInUseProblem
						{
							component = component,
							gameObject = component.gameObject,
							trackableID = trackableID,
							boneName = fieldName,
							trackableManager = trackableManager
						});
					}
					// claim the key
					else
					{
						trackableManager.Register(new TemporaryTrackable(trackableID.value,
							$"{component.gameObject.name} ({fieldName})"));
					}
				}
			}

			return problems;
		}
	}

	public class MetaHandsHookSaveProblemsFinder : FieldBaseIDSaveProblemsFinder
	{
		public MetaHandsHookSaveProblemsFinder() : base(typeof(MetaHandsHook))
		{
		}
	}

	public class MetaControllerHookSaveProblemsFinder : FieldBaseIDSaveProblemsFinder
	{
		public MetaControllerHookSaveProblemsFinder() : base(typeof(MetaRuntimeControllerHook))
		{
		}
	}

	public class MetaOVRCameraHookSaveProblemsFinder : FieldBaseIDSaveProblemsFinder
	{
		public MetaOVRCameraHookSaveProblemsFinder() : base(typeof(MetaOVRCameraHook))
		{
		}
	}


	public class MetaOVRPlayerHookSaveProblemsFinder : FieldBaseIDSaveProblemsFinder
	{
		public MetaOVRPlayerHookSaveProblemsFinder() : base(typeof(MetaOVRPlayerHook))
		{
		}
	}
}