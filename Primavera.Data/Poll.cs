using System;
using System.Collections.Generic;

namespace Primavera.Data
{
    public class Poll
    {
        public string Pollster { get; set; }
        public string State { get; set; }
        public DateTime Date { get; set; }
        public List<PollResult> Results { get; } = new List<PollResult>();
    }
}