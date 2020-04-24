namespace fidelizPlus_back.AppDomain
{
    public class TraderDTO : UserDTO<TraderAccountDTO>
    {
        public string Label { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string LogoPath { get; set; }
    }
}
