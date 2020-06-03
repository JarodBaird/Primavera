using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Primavera.Forecaster;
using Primavera.Parsers.Util;
using Primavera.Results;

namespace Primavera
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var runs = 100000;
            DateTime now = DateTime.Now;

            if (args?.Length > 0)
            {
                int.TryParse(args[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out runs);
            }

            IForecaster forecaster = new PresidentialForecaster();

            var results = new List<ForecastResult>();

            for (var i = 0; i < runs; i++)
            {
                results.Add(forecaster.Forecast(now));
            }

            IEnumerable<IGrouping<string, CandidateResult>> grouped =
                results.SelectMany(r => r.Results).GroupBy(r => r.State);

            var finalResults = new ForecastResult();
            foreach (IGrouping<string, CandidateResult> group in grouped)
            {
                IEnumerable<IGrouping<string, CandidateResult>> subgroups = group.GroupBy(g => g.Candidate);
                foreach (IGrouping<string, CandidateResult> subgroup in subgroups)
                {
                    finalResults.Results.Add(new CandidateResult
                    {
                        Candidate = subgroup.First().Candidate,
                        State = subgroup.First().State,
                        WinPercent = (decimal) subgroup.Count() * 100 / group.Count()
                    });
                }
            }

            foreach (CandidateResult res in finalResults.Results)
            {
                Console.WriteLine($"{res.State}: {res.Candidate} at {res.WinPercent}%");
            }

            FileHelper.OutputToFile("forecast", finalResults);
        }
    }
}