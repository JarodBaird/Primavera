using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Primavera.Data;

namespace Primavera.Parsers.Polls.PollParsers
{
    public class FiveThirtyEight2020Parser : FiveThirtyEightParser
    {
        private readonly string[] _candidates =
        {
            "Joseph R. Biden Jr.",
            "Donald Trump"
        };

        protected override Uri Url => new Uri("https://projects.fivethirtyeight.com/polls-page/president_polls.csv");

        protected override Poll[] ParseFromStream(Stream stream)
        {
            using var reader = new StreamReader(stream);
            var parsedPolls = new List<FiveThirtyEightPoll>();
            reader.ReadLine(); // Skip Header
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line != null)
                {
                    string[] values = line.Split(',');

                    if (int.TryParse(values[(int) CsvMapping.QuestionID], NumberStyles.Integer,
                            CultureInfo.InvariantCulture, out int questionID) &&
                        decimal.TryParse(values[(int) CsvMapping.Pct], NumberStyles.Float, CultureInfo.InvariantCulture,
                            out decimal percent))
                    {
                        parsedPolls.Add(new FiveThirtyEightPoll
                        {
                            QuestionID = questionID,
                            State = values[(int) CsvMapping.State],
                            Pollster = values[(int) CsvMapping.Pollster],
                            Candidate = values[(int) CsvMapping.CandidateName],
                            Percent = percent,
                            EndDate = values[(int) CsvMapping.EndDate]
                        });
                    }
                }
            }

            IEnumerable<IGrouping<int, FiveThirtyEightPoll>> groupedPolls = parsedPolls.GroupBy(p => p.QuestionID);

            var polls = new List<Poll>();

            foreach (IGrouping<int, FiveThirtyEightPoll> p in groupedPolls)
            {
                FiveThirtyEightPoll metadata = p.First();
                var validPoll = true;
                if (p.Count() == _candidates.Length)
                {
                    foreach (string candidate in _candidates)
                    {
                        if (p.All(i => i.Candidate != candidate))
                        {
                            validPoll = false;
                        }
                    }
                }
                else
                {
                    validPoll = false;
                }

                if (validPoll)
                {
                    List<PollResult> results = p.Select(result => new PollResult
                    {
                        Candidate = result.Candidate,
                        Percent = result.Percent
                    }).ToList();

                    var poll = new Poll
                    {
                        Pollster = metadata.Pollster,
                        State = metadata.State,
                        Date = DateTime.Parse(metadata.EndDate, CultureInfo.CurrentCulture)
                    };

                    poll.Results.AddRange(results);

                    polls.Add(poll);
                }
            }

            return polls.ToArray();
        }

        private class FiveThirtyEightPoll
        {
            public int QuestionID { get; set; }
            public string State { get; set; }
            public string Pollster { get; set; }
            public string Candidate { get; set; }
            public decimal Percent { get; set; }
            public string EndDate { get; set; }
        }

        private enum CsvMapping
        {
            QuestionID = 0,
            PollID = 1,
            Cycle,
            State,
            PollsterID,
            Pollster,
            SponsorIDs,
            Sponsors,
            DisplayName,
            PollsterRatingID,
            PollsterRatingName,
            FteGrade,
            SampleSize,
            Population,
            PopulationFull,
            Methodology,
            OfficeType,
            SeatNumber,
            SeatName,
            StartDate,
            EndDate,
            ElectionDate,
            SponsorCandidate,
            Internal,
            Partisan,
            Tracking,
            NationwideBatch,
            RankedChoiceReallocated,
            CreatedAt,
            Notes,
            Url,
            Stage,
            RaceID,
            Answer,
            CandidateName,
            CandidateParty,
            Pct
        }
    }
}