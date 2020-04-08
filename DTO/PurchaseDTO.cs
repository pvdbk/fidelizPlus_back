using System;

namespace fidelizPlus_back.DTO
{
    public class PurchaseDTO
    {
        public int? Id { get; set; }
        public int TraderId { get; set; }
        public int ClientId { get; set; }
        public DateTime? PayingTime { get; set; }
        public decimal Amount { get; set; }
    }
}
