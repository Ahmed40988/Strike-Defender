using StrikeDefender.Application.Common.Interfaces;
using StrikeDefender.Domain.Users;
using System;
using StrikeDefender.Application.Common.Interfaces;
using StrikeDefender.Infrastructure.Common.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Web.Infrastructure.Users.Persistence
{
    public class UserRepository(StrikeDefenderDbContext dbContext) : IUserRepository
    {
        private readonly StrikeDefenderDbContext _dbContext = dbContext;

        public async Task<bool> ExistsAsync(string userId, CancellationToken cancellationToken)
        {
            return await _dbContext.Users.AnyAsync(x => x.Id == userId && !x.Deleted, cancellationToken);
        }

        public async Task<bool> ExistsByEmailAsync(string Email, CancellationToken cancellationToken)
        {
            return await _dbContext.Users.AnyAsync(x => x.Email == Email && !x.Deleted, cancellationToken);
        }
        public async Task<bool> ExistSameEmailandDeletedAsync(string Email, CancellationToken cancellationToken)
        {
            return await _dbContext.Users.AnyAsync(x => x.Email == Email && x.Deleted, cancellationToken);
        }

        public async Task AddAsync(AppUser user, CancellationToken cancellationToken = default)
        {
            await _dbContext.Users.AddAsync(user, cancellationToken);
        }
        public Task DeleteAsync(AppUser user, CancellationToken cancellationToken = default)
        {
            _dbContext.Users.Remove(user);
            _dbContext.UserRoles.RemoveRange(_dbContext.UserRoles.Where(x => x.UserId == user.Id));
            return Task.CompletedTask;
        }

        public async Task<AppUser?> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId && !x.Deleted, cancellationToken);


        }

        public async Task<AppUser?> GetByEmailAsync(string Email, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == Email && !x.Deleted, cancellationToken);
        }

        public async Task<List<AppUser>> ListAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users.Where(x => !x.Deleted).ToListAsync(cancellationToken);
        }



        public async Task UpdateAsync(AppUser user, CancellationToken cancellationToken = default)
        {
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

    }
}
