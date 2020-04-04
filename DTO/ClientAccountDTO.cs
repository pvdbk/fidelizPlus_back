namespace fidelizPlus_back.DTO
{
    public class ClientAccountDTO : DTO
    {
        public int ClientId { get; set; }
        public string ExternalAccount { get; set; }
        public decimal Balance { get; set; }
    }
}
