namespace fidelizPlus_back.AppDomain
{
    public class TraderForClients
    {
        public int? Id { get; set; }
        public string Label { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string LogoPath { get; set; }
        public CommercialRelation CommercialRelation { get; set; }
    }
}
