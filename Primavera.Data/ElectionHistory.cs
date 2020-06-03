using System.Collections.Generic;

namespace Primavera.Data
{
    public class StateElectionHistory
    {
        public string State { get; set; }
        public List<Election> Elections { get; } = new List<Election>();

        public class Election
        {
            public int Year { get; set; }
            public decimal Turnout { get; set; }
            public List<ElectionResult> Results { get; } = new List<ElectionResult>();
        }

        public class ElectionResult
        {
            public string Candidate { get; set; }
            public string Party { get; set; }
            public decimal Pct { get; set; }
        }
    }
}