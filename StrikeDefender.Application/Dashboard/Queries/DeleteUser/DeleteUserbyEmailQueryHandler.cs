
using StrikeDefender.Domain.Users;

namespace StrikeDefender.Application.Dashboard.Queries.DeleteUser
{
    public class DeleteUserbyEmailQueryHandler(IUserRepository userRepository, IUnitOfWork unitOfWork) : IRequestHandler<DeleteUserbyEmailQuery, ErrorOr<Deleted>>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<Deleted>> Handle(DeleteUserbyEmailQuery Query, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(Query.Email);
            if (user == null)
                return Error.NotFound("User.NotFound", "No user found with the given email or ID.");

            await _userRepository.DeleteAsync(user);

            user.MarkAsDeleted();
            await _unitOfWork.CommitChangesAsync();
            return Result.Deleted;
        }
    }
}
