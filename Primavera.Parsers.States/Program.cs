using Primavera.Data;
using Primavera.Parsers.States.StateParsers;
using Primavera.Parsers.Util;

namespace Primavera.Parsers.States
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            IStateParser parser = new CensusBureauStateParser();
            State[] states = parser.GetStatesAsync().Result;
            FileHelper.OutputToFile("States", states);
        }
    }
}