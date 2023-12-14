using Pcg;

namespace EventHorizon
{
	public interface IRandomNumberGenerator
	{
		public int Next();
	}

	public class PcgRng : IRandomNumberGenerator
	{
		private readonly PcgRandom random;
		public PcgRng(int seed = 0x4f6e3a26) => random = new PcgRandom(seed);
		public int Next() => random.Next();
	}
}