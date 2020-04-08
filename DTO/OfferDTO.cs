using System;

namespace fidelizPlus_back.DTO
{
    public class OfferDTO
    {
        public int? TraderId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ContentPath { get; set; }
    }
}
