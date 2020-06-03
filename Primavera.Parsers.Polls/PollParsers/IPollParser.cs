using System.Threading.Tasks;
using Primavera.Data;

namespace Primavera.Parsers.PollParsers
{
    public interface IPollParser
    {
        Task<Poll[]> GetPollsAsync();
    }
}