namespace fidelizPlus_back.DTO
{
    public class ClientOfferDTO
    {
        public int? Id { get; set; }
        public int? ClientId { get; set; }
        public int OfferId { get; set; }
        public int UsedCount { get; set; }
        public int ReceivedCount { get; set; }
    }
}
