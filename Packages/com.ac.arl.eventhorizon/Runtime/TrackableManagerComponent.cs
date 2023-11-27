using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace EventHorizon
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	public sealed class TrackableManagerComponent : MonoBehaviour, ITrackableManager
	{
		private TrackableManager manager;

		private static TrackableManagerComponent instance;
		public static TrackableManagerComponent Instance
		{
			get
			{
				if (instance != null) return instance;

				instance = FindObjectOfType<TrackableManagerComponent>();
				if (instance == null)
					Debug.LogError("An instance of TrackableManager is needed in the scene, but there is none.");

				return instance;
			}
		}

		// public so we could run EventHorizon.Tests.TrackableManagerTests.TestSingletonUniqueness
		public void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(this);
				throw new InvalidOperationException("Another instance of TrackableManager already exists.");
			}

			instance = this;
			manager = new TrackableManager();
			if (Application.isPlaying)
			{
				DontDestroyOnLoad(this.gameObject);
			}
		}

		private void OnDestroy()
		{
			if (Instance == this) instance = null;
		}

		public IReadOnlyDictionary<TrackableID, Trackable> RegisteredTrackables => manager.RegisteredTrackables;
		public void Register(Trackable trackable) => manager.Register(trackable);
		public void Unregister(Trackable trackable) => manager.Unregister(trackable);
		public TrackableID GenerateId() => manager.GenerateId();
	}
}
