using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Primavera.Data;

namespace Primavera.Parsers.Polls.PollParsers
{
    public class FiveThirtyEight2016Parser : FiveThirtyEightParser
    {
        protected override Uri Url =>
            new Uri("https://projects.fivethirtyeight.com/general-model/president_general_polls_2016.csv");

        protected override Poll[] ParseFromStream(Stream stream)
        {
            using var reader = new StreamReader(stream);
            reader.ReadLine();
            var polls = new List<Poll>();
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line != null)
                {
                    string[] values = line.Split(',');

                    var results = new List<PollResult>();

                    if (decimal.TryParse(values[CsvMapping.RawpollClinton], NumberStyles.Float,
                        CultureInfo.InvariantCulture, out decimal clintonPct))
                    {
                        results.Add(new PollResult
                        {
                            Candidate = "Hillary Clinton",
                            Percent = clintonPct
                        });
                    }

                    if (decimal.TryParse(values[CsvMapping.RawpollTrump], NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out decimal trumpPct))
                    {
                        results.Add(new PollResult
                        {
                            Candidate = "Donald Trump",
                            Percent = trumpPct
                        });
                    }

                    if (decimal.TryParse(values[CsvMapping.RawpollJohnson], NumberStyles.Float,
                        CultureInfo.InvariantCulture, out decimal johnsonPct))
                    {
                        results.Add(new PollResult
                        {
                            Candidate = "Gary Johnson",
                            Percent = johnsonPct
                        });
                    }

                    if (decimal.TryParse(values[CsvMapping.RawpollMcmullin], NumberStyles.Float,
                        CultureInfo.InvariantCulture, out decimal mcmullinPct))
                    {
                        results.Add(new PollResult
                        {
                            Candidate = "Evan McMullin",
                            Percent = mcmullinPct
                        });
                    }

                    string state = values[CsvMapping.State].Trim('"');
                    if (state == "U.S.")
                    {
                        state = "";
                    }

                    var poll = new Poll
                    {
                        Pollster = values[CsvMapping.Pollster].Trim('"'),
                        State = state,
                        Date = DateTime.Parse(values[CsvMapping.EndDate], CultureInfo.CurrentCulture)
                    };
                    poll.Results.AddRange(results);
                    polls.Add(poll);
                }
            }

            return polls.ToArray();
        }

        private static class CsvMapping
        {
            public const int Cycle = 0;
            public const int Branch = 1;
            public const int Type = 2;
            public const int Matchup = 3;
            public const int ForecastDate = 4;
            public const int State = 5;
            public const int StartDate = 6;
            public const int EndDate = 7;
            public const int Pollster = 8;
            public const int Grade = 9;
            public const int SampleSize = 10;
            public const int Population = 11;
            public const int PollWeight = 12;
            public const int RawpollClinton = 13;
            public const int RawpollTrump = 14;
            public const int RawpollJohnson = 15;
            public const int RawpollMcmullin = 16;
            public const int AdjpollClinton = 17;
            public const int AdjpollTrump = 18;
            public const int AdjpollJohnson = 19;
            public const int AdjpollMcmullin = 20;
            public const int Multiversions = 21;
            public const int Url = 22;
            public const int PollID = 23;
            public const int QuestionID = 24;
            public const int CreatedDate = 25;
            public const int Timestamp = 26;
        }
    }
}