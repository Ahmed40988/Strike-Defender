using Microsoft.AspNetCore.Identity;
namespace StrikeDefender.Infrastructure.Users.Persistance
{
    public class AppUser :IdentityUser
    {
        public  Guid DomainUserId { get; private set; }
        public string FullName { get; set; } = string.Empty;
        public string? PhotoURl { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }

        private AppUser() { }

        public AppUser(string email)
        {
            
            Email = email;
            UserName = email;
        }


    }
}


