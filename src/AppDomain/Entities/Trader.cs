using System.Collections.Generic;

namespace fidelizPlus_back.AppDomain
{
	public partial class Trader : UserEntity<TraderAccount>
	{
		public Trader()
		{
			CommercialLink = new HashSet<CommercialLink>();
			Offer = new HashSet<Offer>();
		}

		public string Label { get; set; }
		public string Address { get; set; }
		public string Phone { get; set; }

		public virtual ICollection<Offer> Offer { get; set; }
	}
}
