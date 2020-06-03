using System.Threading.Tasks;
using Primavera.Data;

namespace Primavera.Parsers.States
{
    public interface IStateParser
    {
        Task<State[]> GetStatesAsync();
    }
}