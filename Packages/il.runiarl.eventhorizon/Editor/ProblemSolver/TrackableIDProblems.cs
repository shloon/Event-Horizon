namespace EventHorizon.Editor.ProblemSolver
{
	public struct InvalidTrackableIDProblem : IProblem
	{
		public Trackable trackable;
		public ITrackableManager trackableManager;

		public string Description => $"GameObject \"{trackable.gameObject.name}\" has no valid ID assigned to it";

		public void Fix()
		{
			trackable.id = trackableManager.GenerateId();
			trackableManager.Register(trackable);
		}
	}

	public struct TwoTrackablesWithSameIDProblem : IProblem
	{
		public Trackable trackable;
		public Trackable otherTrackable;
		public ITrackableManager trackableManager;

		public string Description =>
			$"GameObject \"{trackable.gameObject.name}\"'s ID is already assigned to \"{otherTrackable.gameObject.name}\"";

		public void Fix()
		{
			trackable.id = trackableManager.GenerateId();
			trackableManager.Register(trackable);
		}
	}
}