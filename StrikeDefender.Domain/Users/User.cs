using ErrorOr;
namespace StrikeDefender.Domain.Users
{
    public class User
    {
        public Guid Id { get; private set; }

        private User() { }

        public User(Guid id)
        {
            Id = id;
        }

    }
}