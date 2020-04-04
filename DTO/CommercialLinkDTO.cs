namespace fidelizPlus_back.DTO
{
    public class CommercialLinkDTO : DTO
    {
        public int? TraderId { get; set; }
        public int? ClientId { get; set; }
        public ClType Flags { get; set; }
    }
}
