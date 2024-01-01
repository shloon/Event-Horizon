namespace EventHorizon
{
	public interface ITrackable
	{
		TrackableID Id { get; set; }
		string Name { get; }
	}
}