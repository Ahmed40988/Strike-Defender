namespace StrikeDefender.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        Task CommitChangesAsync();

    }
}
