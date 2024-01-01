using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EventHorizon
{
	/*
	 * TrackableManagerComponent is central to the Event Horizon system.
	 * We've employed the singleton pattern as, ensuring that the system
	 * 	remains intuitive for developers while robust against Unity's unique behaviors.
	 *
	 * Desired Behaviors:
	 * 1. Robustness: It works seamlessly in both edit mode and play mode, handling
	 *    Unity-specific scenarios gracefully.
	 * 2. Uniqueness: Maintains only one instance in a scene to avoid conflicts.
	 * 3. Persistence: Keeps state consistent across Unity scene transitions.
	 * 4. Ease of Use: Simple for users (just add the component), while abstracting away
	 *    the complexities of Unity's lifecycle.
	 *
	 * Unity Quirks and Workarounds:
	 * Implementing a GameObject-attached singleton in Unity is challenging due to its reliance on
	 * MonoBehaviours and their lifecycle, along with frequent domain reloads (script recompilation, play mode changes).
	 *
	 * Our strategy to navigate these challenges includes:
	 * - Utilizing MonoBehavior lifecycle methods (Awake, OnEnable, OnDestroy) over typical .NET
	 *   constructors, ensuring alignment with Unity's instantiation and destruction flow.
	 * - Employing `[SerializeField]` for state retention across domain reloads.
	 * - Using `[DefaultExecutionOrder(-100)]` to ensure early initialization of our component.
	 * - Implementing `DontDestroyOnLoad` for continuous presence across scenes.
	 * - Supporting our design with a comprehensive suite of unit tests for added reliability.
	 *
	 * Lifecycle Methods:
	 * - `Awake`: Initializes the singleton, effective in active GameObjects. This is where
	 *    we first set up the instance.
	 *
	 * - `OnEnable`: Activated post-`Awake` or when the GameObject is re-enabled. Vital for
	 *    re-validating the singleton after domain reloads. Critical because `Awake` doesn't
	 *    re-run for merely disabled and re-enabled GameObjects.
	 *
	 * - `OnDestroy`: Responsible for cleanup, especially crucial for resetting the static
	 *    instance if the current instance is being decommissioned.
	 *
	 * - `[RuntimeInitializeOnLoadMethod]`: A preemptive static method invoked before any scene
	 *    loads, ensuring the singleton's integrity, especially after domain reloads.
	 */
	[AddComponentMenu("Event Horizon/Trackable Manager")]
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[DefaultExecutionOrder(-100)]
	public sealed class TrackableManagerComponent : MonoBehaviour, ITrackableManager
	{
		private TrackableManager manager;

		public IReadOnlyDictionary<TrackableID, ITrackable> RegisteredTrackables => manager.RegisteredTrackables;
		public void Register(ITrackable trackable) => manager.Register(trackable);
		public void Unregister(ITrackable trackable) => manager.Unregister(trackable);
		public void ChangeTrackableID(TrackableID previousID, TrackableID newID) =>
			manager.ChangeTrackableID(previousID, newID);
		
		public TrackableID GenerateId() => manager.GenerateId();

		#region Singleton Handling

		private static TrackableManagerComponent instance;

		public static TrackableManagerComponent Instance
		{
			get
			{
				if (instance != null)
				{
					return instance;
				}

				instance = FindObjectOfType<TrackableManagerComponent>();
				if (instance == null)
				{
					throw new NullReferenceException(
						"An instance of TrackableManager is needed in the scene, but there is none.");
				}

				return instance;
			}
		}

		// public so we could run EventHorizon.Tests.TrackableManagerTests.TestSingletonUniqueness
		public void Awake()
		{
			ValidateSingleton();
			InitializeManager();
		}

		private void OnEnable()
		{
			ValidateSingleton();
			InitializeManager();
		}

		private void OnDestroy()
		{
#if UNITY_EDITOR
			// detect if we're just about to play
			// see https://gist.github.com/JakubNei/74bb5eba12a91a7d5f6334e7af365b11
			if ((!EditorApplication.isPlayingOrWillChangePlaymode && Time.frameCount == 0) ||
				Time.renderedFrameCount == 0)
			{
				return;
			}
#endif

			if (Instance == this)
			{
				instance = null;
			}
		}

		private void ValidateSingleton()
		{
			if (instance != null && instance != this)
			{
				Destroy(this);
				throw new InvalidOperationException("Another instance of TrackableManager already exists.");
			}

			instance = this;
			if (Application.isPlaying)
			{
				DontDestroyOnLoad(gameObject);
			}
		}

		private void InitializeManager() => manager ??= new TrackableManager();

#if UNITY_EDITOR
		[InitializeOnLoadMethod]
		private static void OnAfterSceneLoadRuntimeMethod()
		{
			// see if there's anything in the scene
			if (FindObjectOfType<TrackableManagerComponent>() == null)
			{
				return;
			}

			// Accessing the Instance property will automatically retrieve the TrackableManagerComponent instance if need be.
			instance = Instance;
			instance.InitializeManager();
		}
#endif

		#endregion
	}
}