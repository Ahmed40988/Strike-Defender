using Microsoft.EntityFrameworkCore;

namespace StrikeDefender.Domain.RefreshTokens
{
    [Owned]
    public class RefreshToken
    {
        public string Token { get; private set; } = string.Empty;
        public DateTime ExpiresOn { get; private set; }
        public DateTime CreatedOn { get; private set; }
        public DateTime? RevokedOn { get; private set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
        public bool IsActive => RevokedOn is null && !IsExpired;

        private RefreshToken() { }

        public RefreshToken(string token, DateTime expiresOn)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token cannot be empty.", nameof(token));

            if (expiresOn <= DateTime.UtcNow)
                throw new ArgumentException("Expiration must be in the future.", nameof(expiresOn));

            Token = token;
            ExpiresOn = expiresOn;
            CreatedOn = DateTime.UtcNow;
        }

        public static RefreshToken Create(string token, DateTime expiresOn)
        {
            return new RefreshToken(token, expiresOn);
        }

        public void Revoke()
        {
            if (RevokedOn is not null)
                return;

            RevokedOn = DateTime.UtcNow;
        }
    }
}
