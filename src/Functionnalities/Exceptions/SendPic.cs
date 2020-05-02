namespace fidelizPlus_back
{
	public class SendPic : System.Exception
	{
		public byte[] PicBytes { get; }

		public SendPic(byte[] picBytes) : base() => PicBytes = picBytes;
	}
}
