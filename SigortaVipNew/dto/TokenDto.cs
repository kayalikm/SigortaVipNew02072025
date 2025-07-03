namespace SigortaVip.Dto
{
    public class TokenDto
    {
        public string token { get; set; }
        public bool isAdmin { get; set; }
        public bool isSuperAdmin { get; set; }
        public LicenceDto licence { get; set; }
    }
}
