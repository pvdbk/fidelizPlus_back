﻿using System;

namespace fidelizPlus_back.DTO
{
    public class PurchaseDTO : RelatedToBothDTO
    {
        public int Id { get; set; }
        public DateTime? PayingTime { get; set; }
        public decimal Amount { get; set; }
    }
}
