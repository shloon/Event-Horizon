using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace EventHorizon
{
	public sealed class TrackableManagerInternal : ITrackableManager
	{
		private Dictionary<TrackableID, Trackable> registeredTrackables;
		private const int MaxGenerateAttempts = 128;

		public IReadOnlyDictionary<TrackableID, Trackable> RegisteredTrackables => registeredTrackables;

		public TrackableManagerInternal()
		{
			registeredTrackables = new Dictionary<TrackableID, Trackable>();
		}
		
		public void Register(Trackable trackable)
		{
			if (!trackable.id.IsValid)
				throw new ArgumentException($"Trackable '{trackable.gameObject}' has invalid key");

			if (registeredTrackables.TryGetValue(trackable.id, out Trackable existingTrackable))
			{
				if (existingTrackable == trackable)
					throw new InvalidOperationException($"Trackable '{trackable.gameObject}' already registered");

				throw new InvalidOperationException($"Trackable '{trackable.gameObject}' has key registered by another trackable");
			}

			registeredTrackables.Add(trackable.id, trackable);
		}

		public void Unregister(Trackable trackable)
		{
			if (!trackable.id.IsValid)
				throw new ArgumentException($"Trackable '{trackable.gameObject}' has invalid key");

			if (!registeredTrackables.TryGetValue(trackable.id, out Trackable existingTrackable))
				throw new InvalidOperationException($"Trackable '{trackable.gameObject}' is not registered");

			if (existingTrackable != trackable)
				throw new InvalidOperationException($"Attempt to remove '{trackable.gameObject}' would remove another trackable with the same ID");

			registeredTrackables.Remove(trackable.id);
		}

		public TrackableID GenerateId()
		{
			return new TrackableID();
		}
	}

	[ExecuteAlways]
    [DisallowMultipleComponent]
    public sealed class TrackableManager : MonoBehaviour, ITrackableManager
    {
	    private TrackableManagerInternal internalManager;

	    private static TrackableManager instance;
	    public static TrackableManager Instance
	    {
		    get
		    {
			    if (instance != null) return instance;

			    instance = FindObjectOfType<TrackableManager>();
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
            internalManager = new TrackableManagerInternal();
            if (Application.isPlaying)
            {
	            DontDestroyOnLoad(this.gameObject);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this) instance = null;
        }

        public IReadOnlyDictionary<TrackableID, Trackable> RegisteredTrackables => internalManager.RegisteredTrackables;
        public void Register(Trackable trackable) => internalManager.Register(trackable);
        public void Unregister(Trackable trackable) => internalManager.Unregister(trackable);
        public TrackableID GenerateId() => internalManager.GenerateId();
    }

    public interface ITrackableManager
    {
	    public IReadOnlyDictionary<TrackableID, Trackable> RegisteredTrackables { get; }
        void Register(Trackable trackable);
        void Unregister(Trackable trackable);
        public TrackableID GenerateId();
    }
}
