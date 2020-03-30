using System;
using System.Collections.Generic;

namespace fidelizPlus_back.Models
{
    public partial class Comment : Entity
    {
        public override IEnumerable<string> Fields => new string[] { "Id", "CommercialLinkId", "CreationTime", "Text", "Rating" };
        public int CommercialLinkId { get; set; }
        public DateTime CreationTime { get; set; }
        public string Text { get; set; }
        public int? Rating { get; set; }

        public virtual CommercialLink CommercialLink { get; set; }
    }
}
