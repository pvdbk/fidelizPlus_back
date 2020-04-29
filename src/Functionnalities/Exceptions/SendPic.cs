namespace fidelizPlus_back
{
    public class SendPic : AppException
    {
        public SendPic(byte[] picBytes) : base(picBytes) { }
    }
}