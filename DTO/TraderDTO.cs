namespace fidelizPlus_back.DTO
{
    public class TraderDTO : UserDTO
    {
        public string Label { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string LogoPath { get; set; }
    }
}
