using Primavera.Data;
using Primavera.Mutators;
using Primavera.Results.Partial;
using System;
using System.Linq;
using Xunit;

namespace Primavera.Tests.Unit.Mutators
{
    public class PartyTurnoutMutatorTests
    {
        [Fact]
        public void Mutate_ZeroVotes()
        {
            var mutator = new PartyTurnoutMutator(Party.Democratic, .05M);
            var result = mutator.Mutate(new StateElectionResult(
                new[]
                {
                    new ElectionResult
                    {
                        Candidate = "Democrat",
                        Party = Party.Democratic,
                        Pct = 0
                    },
                    new ElectionResult
                    {
                        Candidate = "Republican",
                        Party = Party.Republican,
                        Pct = 0
                    }
                })
            {
                State = "TestState"
            });

            Assert.Equal(0, result.Results.ElementAt(0).Pct);
            Assert.Equal(0, result.Results.ElementAt(1).Pct);
        }

        [Fact]
        public void Mutate_NoCandidate()
        {
            throw new NotImplementedException();
        }
    }
}
