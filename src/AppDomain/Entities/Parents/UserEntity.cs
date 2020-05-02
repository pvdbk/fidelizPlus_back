using System.Collections.Generic;

namespace fidelizPlus_back.AppDomain
{
	public class UserEntity<TAccount> : Entity
	{
		public int UserId { get; set; }
		public int AccountId { get; set; }
		public string ConnectionId { get; set; }
		public User User { get; set; }
		public TAccount Account { get; set; }
		public ICollection<CommercialLink> CommercialLink { get; set; }
	}
}
