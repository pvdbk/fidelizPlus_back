using System;
using System.Collections.Generic;

namespace fidelizPlus_back.AppDomain
{
	public partial class Offer : Entity
	{
		public Offer()
		{
			ClientOffer = new HashSet<ClientOffer>();
		}

		public int? TraderId { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public string ContentPath { get; set; }

		public virtual Trader Trader { get; set; }
		public virtual ICollection<ClientOffer> ClientOffer { get; set; }
	}
}
