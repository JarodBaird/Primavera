using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Primavera.Data;
using Primavera.Math;
using Primavera.Results;

namespace Primavera.Forecaster
{
    public class PresidentialForecaster : IForecaster
    {
        public ForecastResult Forecast(DateTime forecastDate)
        {
            int year = forecastDate.Year;

            State[] states = GetStates().ToArray();
            Poll[] polls = GetPolls(forecastDate).ToArray();
            Candidate[] candidates = GetCandidates(year).ToArray();
            StateElectionHistory[] history = GetElectionHistory().ToArray();

            (decimal partisanSwing, string currentPartisanship) =
                GenerateSwing(forecastDate, GetElectionDate(forecastDate));

            Dictionary<string, int> votes = candidates.ToDictionary(candidate => candidate.Name, candidate => 0);

            var finalResult = new ForecastResult();

            foreach (State state in states)
            {
                StateElectionHistory stateHistory = history.FirstOrDefault(h => h.State == state.Name);
                if (stateHistory != null)
                {
                    IEnumerable<StateElectionHistory.Election> elections =
                        stateHistory.Elections.Where(e => e.Year < year).ToArray();

                    var stateLean = new Dictionary<string, decimal>();
                    foreach (Candidate candidate in candidates)
                    {
                        decimal runningTotal = 0;
                        decimal totalWeight = 0;
                        foreach (StateElectionHistory.Election election in elections)
                        {
                            var weight = (decimal)System.Math.Pow(10.0 / (year - election.Year), 2);
                            totalWeight += weight;
                            runningTotal +=
                                (election.Results.FirstOrDefault(r => r.Party == candidate.Party)?.Pct ?? 0) * weight;
                        }

                        Poll[] statePolls = polls.Where(p => p.State == state.Name).ToArray();
                        if (statePolls.Any())
                        {
                            foreach (Poll poll in statePolls)
                            {
                                (decimal result, decimal weight) = GetGuassian(poll, candidate.Name, forecastDate);
                                runningTotal += result;
                                totalWeight += weight;
                            }
                        }

                        decimal endPct = runningTotal / totalWeight;
                        endPct += currentPartisanship == candidate.Party ? partisanSwing : -partisanSwing;

                        stateLean.Add(candidate.Party, endPct);
                    }

                    string winningParty = stateLean.FirstOrDefault(l => l.Value == stateLean.Values.Max()).Key;
                    Candidate winner = candidates.FirstOrDefault(c => c.Party == winningParty);
                    if (winner != null)
                    {
                        votes[winner.Name] += state.EcVotes;

                        finalResult.Results.Add(new CandidateResult
                        {
                            State = state.Name,
                            Candidate = winner.Name,
                            WinPercent = 100
                        });
                    }
                }
            }

            finalResult.Results.Add(new CandidateResult
            {
                State = "",
                Candidate = votes.FirstOrDefault(v => v.Value == votes.Values.Max()).Key,
                WinPercent = 100
            });

            return finalResult;
        }

        private static IEnumerable<State> _states;
        private static IEnumerable<State> GetStates()
        {
            if (_states == null)
            {
                string statesPath = Path.Join(Directory.GetCurrentDirectory(), "Data", "States", "States.json");
                string json = File.ReadAllText(statesPath);
                State[] states = JsonConvert.DeserializeObject<State[]>(json);
                _states = states;
            }

            return _states;
        }

        private static IEnumerable<StateElectionHistory> _electionHistory;
        private static IEnumerable<StateElectionHistory> GetElectionHistory()
        {
            if (_electionHistory == null)
            {
                string electionHistory = Path.Join(Directory.GetCurrentDirectory(), "Data", "Elections");
                string[] files = Directory.GetFiles(electionHistory);

                _electionHistory = files.Select(File.ReadAllText)
                    .Select(JsonConvert.DeserializeObject<StateElectionHistory>)
                    .ToArray();
            }

            return _electionHistory;
        }

        private static IEnumerable<Poll> _polls;
        private static IEnumerable<Poll> GetPolls(DateTime forecastDate)
        {
            if (_polls == null)
            {
                string pollsPath = Path.Join(Directory.GetCurrentDirectory(), "Data", "Polls", $"{forecastDate.Year}.json");
                string json = File.ReadAllText(pollsPath);
                Poll[] polls = JsonConvert.DeserializeObject<Poll[]>(json);
                _polls = polls.Where(p => p.Date <= forecastDate).ToArray();
            }

            return _polls;
        }

        private static IEnumerable<Candidate> _candidates;
        private static IEnumerable<Candidate> GetCandidates(int year)
        {
            if (_candidates == null)
            {
                string candidatesPath = Path.Join(Directory.GetCurrentDirectory(), "Data", "Candidates", $"{year}.json");
                string json = File.ReadAllText(candidatesPath);
                _candidates = JsonConvert.DeserializeObject<Candidate[]>(json);
            }

            return _candidates;
        }

        private static (decimal Result, decimal Weight) GetGuassian(Poll poll, string candidateName, DateTime date)
        {
            PollResult result = poll.Results.FirstOrDefault(c => c.Candidate == candidateName);
            if (result != null)
            {
                decimal mean = result.Percent;
                decimal stdDev = ((decimal)(date - poll.Date).Days / 60) + 1;
                decimal pct = Guassian.Next(mean, stdDev);
                int denominator = (date - poll.Date).Days;
                if (denominator == 0)
                {
                    denominator = 1;
                }

                var weight = (decimal)System.Math.Sqrt(1.0 / denominator);
                return (pct * weight, weight);
            }

            return (0.0M, 0.0M);
        }

        private static (decimal Swing, string Party) GenerateSwing(DateTime forecastDate, DateTime electionDay)
        {
            string party = new Random().Next() % 2 == 0 ? "Republican" : "Democratic";
            decimal swing = Guassian.Next(0, ((decimal)(electionDay - forecastDate).Days / 90) + 1);

            return (swing, party);
        }

        private static DateTime GetElectionDate(DateTime forecastDate)
        {
            return forecastDate.Year switch
            {
                2020 => new DateTime(2020, 11, 3),
                2016 => new DateTime(2016, 11, 8),
                _ => DateTime.Now
            };
        }
    }
}