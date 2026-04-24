using ErrorOr;
using Microsoft.AspNetCore.Identity;
using StrikeDefender.Domain.RefreshTokens;
using StrikeDefender.Domain.Subscriptions;
using System.Net;

namespace StrikeDefender.Domain.Users
{
    public class AppUser :IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? PhotoURl { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }

       
        public bool Deleted { get; private set; }
        public DateTime? DeletedOn { get; private set; }
        public DateTime CreatedOn { get; private set; } = DateTime.UtcNow;
        public string? UpdatedByid { get; private set; }
        public DateTime? Updatedon { get; private set; }
        public Guid SubscriptionId { get; private set; }

        private readonly List<RefreshToken> _refreshTokens = new();
        public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
        private AppUser() { }

        public AppUser(string email)
        {
            
            Email = email;
            UserName = email;
        }

        public void UpdateProfile(
            string fullName,
            string phone,
            DateOnly dateOfBirth)
                    {
            FullName = fullName;
            PhoneNumber = phone;
            DateOfBirth = dateOfBirth;
        }

        // ─── Identity-specific behavior ───────────────────────

        protected void Touch(string updatedById)
        {
            UpdatedByid = updatedById;
            Updatedon = DateTime.UtcNow;
        }
        public void MarkAsDeleted()
        {
            if (Deleted) return;
            Deleted = true;
            DeletedOn = DateTime.UtcNow;
        }

        public void Restore(string updatedById)
        {
            if (!Deleted) return;

            Deleted = false;
            Touch(updatedById);
        }

        public ErrorOr<Success> AddrefreshTokens(RefreshToken refreshToken)
        {
            if (_refreshTokens.Contains(refreshToken))
                return RefreshTokensErrors.DuplicatedRefreshToken;

            if (refreshToken is null)
                return RefreshTokensErrors.RefreshTokenIsnulll;

            _refreshTokens.Add(refreshToken);
            return Result.Success;
        }
   
        public void RevokeRefreshToken(string token)
        {
            var refreshToken = _refreshTokens.FirstOrDefault(t => t.Token == token);
            refreshToken?.Revoke();
        }

        public void RevokeAllRefreshTokens()
        {
            foreach (var token in _refreshTokens)
                token.Revoke();
        }

        public void setSubscrition(Subscription subscription, string updatedById)
        {
            SubscriptionId = subscription.Id;
            Touch(updatedById);
        }
        public void UpdateSubscription(Subscription subscription, string updatedById)
        {
            SubscriptionId = subscription.Id;  
            Touch(updatedById);
        }

    }
}


