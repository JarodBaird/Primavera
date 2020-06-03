using System;
using System.Globalization;
using Primavera.Data;
using Primavera.Parsers.PollParsers;
using Primavera.Parsers.Polls.PollParsers;
using Primavera.Parsers.Util;

namespace Primavera.Parsers.Polls
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var year = 2020;
            if (args?.Length > 0)
            {
                int.TryParse(args[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out year);
            }

            IPollParser parser = ParserFactory.GetParser(year);
            Poll[] polls = parser.GetPollsAsync().Result;

            FileHelper.OutputToFile(year.ToString(CultureInfo.InvariantCulture), polls);

            Console.WriteLine($"{polls.Length} polls found.");
        }
    }
}