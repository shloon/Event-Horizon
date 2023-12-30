using System;
using System.Collections.Generic;

namespace EventHorizon
{
	public interface ITrackableManager
	{
		public IReadOnlyDictionary<TrackableID, Trackable> RegisteredTrackables { get; }
		void Register(Trackable trackable);
		void Unregister(Trackable trackable);
		void ChangeTrackableID(TrackableID previousID, TrackableID newID);
		TrackableID GenerateId();
	}

	public sealed class TrackableManager : ITrackableManager
	{
		private readonly Dictionary<TrackableID, Trackable> registeredTrackables = new();
		public IReadOnlyDictionary<TrackableID, Trackable> RegisteredTrackables => registeredTrackables;

		private readonly IRandomNumberGenerator rngProvider;

		public TrackableManager(IRandomNumberGenerator provider = null) => rngProvider = provider ?? new PcgRng();

		public void Register(Trackable trackable)
		{
			if (!trackable.id.IsValid)
				throw new ArgumentException($"Trackable '{trackable.gameObject}' has invalid key");

			if (registeredTrackables.TryGetValue(trackable.id, out Trackable existingTrackable))
			{
				if (existingTrackable == trackable)
					throw new InvalidOperationException($"Trackable '{trackable.gameObject}' already registered");

				throw new InvalidOperationException(
					$"Trackable '{trackable.gameObject}' has key registered by another trackable");
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
				throw new InvalidOperationException(
					$"Attempt to remove '{trackable.gameObject}' would remove another trackable with the same ID");

			registeredTrackables.Remove(trackable.id);
		}

		public void ChangeTrackableID(TrackableID previousID, TrackableID newID)
		{
			if (previousID == newID) return;			
			
			if (!registeredTrackables.TryGetValue(previousID, out var existingObject))
				throw new ArgumentException($"Could not find trackable with ID '{previousID}'");

			if (registeredTrackables.TryGetValue(newID, out var objectWithSameID))
				throw new InvalidOperationException(
					$"Could not change to the ID above since `{objectWithSameID}` already uses it");

			registeredTrackables.Remove(previousID);
			registeredTrackables.Add(newID, existingObject);
		}

		public const int MaxGenerateAttempts = 128;

		public TrackableID GenerateId()
		{
			var attempts = 0;
			while (attempts < MaxGenerateAttempts)
			{
				var identity = new TrackableID((uint) rngProvider.Next());
				if (identity.IsValid && !registeredTrackables.ContainsKey(identity))
					return identity;

				attempts++;
			}

			throw new InvalidOperationException(
				$"Failed to generate a unique ID after {MaxGenerateAttempts} attempts");
		}
	}
}