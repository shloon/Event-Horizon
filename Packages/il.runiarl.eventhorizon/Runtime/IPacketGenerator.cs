using EventHorizon.FileFormat;

namespace EventHorizon
{
	public interface IPacketGenerator<out T> where T : IPacket
	{
		public T GetPacketForFrame(ulong frame);
	}
}