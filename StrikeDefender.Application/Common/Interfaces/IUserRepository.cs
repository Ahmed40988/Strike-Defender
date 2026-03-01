using StrikeDefender.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.Common.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> ExistsAsync(string userId, CancellationToken cancellationToken);
        Task<bool> ExistsByEmailAsync(string Email, CancellationToken cancellationToken);
        Task<bool> ExistSameEmailandDeletedAsync(string Email, CancellationToken cancellationToken);

        Task AddAsync(AppUser user, CancellationToken cancellationToken = default);
        Task DeleteAsync(AppUser user, CancellationToken cancellationToken = default);
        Task<AppUser?> GetByIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<AppUser?> GetByEmailAsync(string Email, CancellationToken cancellationToken = default);
        Task<List<AppUser>> ListAsync(CancellationToken cancellationToken = default);


        Task UpdateAsync(AppUser user, CancellationToken cancellationToken = default);
    }
    }
