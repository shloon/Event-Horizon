using System;
using System.Collections.Generic;

namespace EventHorizon
{
	public interface ITrackableManager
	{
		public IReadOnlyDictionary<TrackableID, ITrackable> RegisteredTrackables { get; }
		void Register(ITrackable trackable);
		void Unregister(ITrackable trackable);
		void ChangeTrackableID(TrackableID previousID, TrackableID newID);
		TrackableID GenerateId();
	}

	public sealed class TrackableManager : ITrackableManager
	{
		private readonly Dictionary<TrackableID, ITrackable> registeredTrackables = new();
		public IReadOnlyDictionary<TrackableID, ITrackable> RegisteredTrackables => registeredTrackables;

		private readonly IRandomNumberGenerator rngProvider;

		public TrackableManager(IRandomNumberGenerator provider = null) => rngProvider = provider ?? new PcgRng();

		public void Register(ITrackable trackable)
		{
			if (!trackable.Id.IsValid)
				throw new ArgumentException($"Trackable '{trackable.Name}' has invalid key");

			if (registeredTrackables.TryGetValue(trackable.Id, out ITrackable existingTrackable))
			{
				if (existingTrackable == trackable)
					throw new InvalidOperationException($"Trackable '{trackable.Name}' already registered");

				throw new InvalidOperationException(
					$"Trackable '{trackable.Name}' has key registered by another trackable");
			}

			registeredTrackables.Add(trackable.Id, trackable);
		}

		public void Unregister(ITrackable trackable)
		{
			if (!trackable.Id.IsValid)
				throw new ArgumentException($"Trackable '{trackable.Name}' has invalid key");

			if (!registeredTrackables.TryGetValue(trackable.Id, out ITrackable existingTrackable))
				throw new InvalidOperationException($"Trackable '{trackable.Name}' is not registered");

			if (existingTrackable != trackable)
				throw new InvalidOperationException(
					$"Attempt to remove '{trackable.Name}' would remove another trackable with the same ID");

			registeredTrackables.Remove(trackable.Id);
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