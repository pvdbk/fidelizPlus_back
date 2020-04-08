using System.Collections.Generic;

namespace fidelizPlus_back.AppModel
{
    public partial class CommercialLink : Entity
    {
        public const int DEFAULT_STATUS = 0;
        public const int BOOKMARK = 0;

        public CommercialLink()
        {
            Comment = new HashSet<Comment>();
            Purchase = new HashSet<Purchase>();
        }

        public int? TraderId { get; set; }
        public int? ClientId { get; set; }
        public int Status { get; set; }

        public virtual Client Client { get; set; }
        public virtual Trader Trader { get; set; }
        public virtual ICollection<Comment> Comment { get; set; }
        public virtual ICollection<Purchase> Purchase { get; set; }
    }
}
