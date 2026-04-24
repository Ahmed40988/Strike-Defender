using StrikeDefender.Domain.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.Common.Interfaces
{
    public interface IPaymentTransactionRepository
    {
        Task<PaymentTransaction?> GetByOrderIdAsync(int orderId, CancellationToken ct = default);
        Task<IReadOnlyList<PaymentTransaction>> GetByUserIdAsync(string userId, CancellationToken ct = default);
    }
}
