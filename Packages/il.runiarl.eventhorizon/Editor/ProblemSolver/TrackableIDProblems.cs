namespace EventHorizon.Editor.ProblemSolver
{
	public struct InvalidTrackableIDProblem : IProblem
	{
		public ITrackable trackable;
		public ITrackableManager trackableManager;

		public string Description => $"GameObject \"{trackable.Name}\" has no valid ID assigned to it";

		public void Fix()
		{
			trackable.Id = trackableManager.GenerateId();
			trackableManager.Register(trackable);
		}
	}

	public struct TwoTrackablesWithSameIDProblem : IProblem
	{
		public ITrackable trackable;
		public ITrackable otherTrackable;
		public ITrackableManager trackableManager;

		public string Description =>
			$"GameObject \"{trackable.Name}\"'s ID is already assigned to \"{otherTrackable.Name}\"";

		public void Fix()
		{
			trackable.Id = trackableManager.GenerateId();
			trackableManager.Register(trackable);
		}
	}
}