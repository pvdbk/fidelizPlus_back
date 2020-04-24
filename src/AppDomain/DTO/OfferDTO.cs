using System;

namespace fidelizPlus_back.AppDomain
{
    public class OfferDTO
    {
        public int? TraderId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ContentPath { get; set; }
    }
}
