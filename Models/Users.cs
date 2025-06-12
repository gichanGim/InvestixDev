using Microsoft.AspNetCore.Identity;

namespace InvestixDev.Models
{
    public class Users : IdentityUser
    {
        public string Phone {  get; set; }
        public string UserId { get; set; }

        public string AppKey { get; set; }
        public string AppSecret { get; set; }
    }
}
