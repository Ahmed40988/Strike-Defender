namespace StrikeDefender.Application.Dashboard.Queries.DeleteUser
{
    public record DeleteUserbyEmailQuery( string Email) : IRequest<ErrorOr<Deleted>>;
}
