using System;

namespace fidelizPlus_back.Models
{
    public partial class Comment : Entity
    {
        public int CommercialLinkId { get; set; }
        public DateTime Moment { get; set; }
        public string Text { get; set; }
        public int? Rating { get; set; }

        public virtual CommercialLink CommercialLink { get; set; }
    }
}
