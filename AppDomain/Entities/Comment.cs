using System;

namespace fidelizPlus_back.AppDomain
{
    public partial class Comment : PurchaseComment
    {
        public DateTime CreationTime { get; set; }
        public string Text { get; set; }
        public int Rating { get; set; }
    }
}
