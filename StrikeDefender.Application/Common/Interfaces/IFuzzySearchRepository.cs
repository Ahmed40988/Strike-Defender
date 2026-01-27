
namespace StrikeDefender.Application.Common.Interfaces
{
    public interface IFuzzySearchRepository
    {
        int CalculateSimilarity(string source, string target);
    }

}
