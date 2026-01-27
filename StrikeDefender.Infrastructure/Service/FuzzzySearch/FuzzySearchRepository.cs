using FuzzySharp;
using StrikeDefender.Application.Common.Interfaces;
namespace StrikeDefender.Infrastructure.Service.FuzzzySearch
{
    public class FuzzySearchRepository : IFuzzySearchRepository
    {
        public int CalculateSimilarity(string source, string target)
        {
            return Fuzz.Ratio(source, target);
        }
    }

}
