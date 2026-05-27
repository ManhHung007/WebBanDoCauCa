using Microsoft.AspNetCore.Identity;

namespace WebBanDoCauCa.Models
{
    // Chỉ kế thừa IdentityUser, KHÔNG được có gì khác gây vòng lặp
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Address { get; set; }
    }
}