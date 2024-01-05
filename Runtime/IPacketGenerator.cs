using EventHorizon.FormatV2;

namespace EventHorizon
{
	public interface IPacketGenerator<out T> where T : IPacket
	{
		public T GetPacketForFrame(ulong frame);
	}
}