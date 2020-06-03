using System.Collections.Generic;
using System.Linq;

namespace Primavera.Data
{
    public class State
    {
        public string Abbreviation { get; set; }
        public string Name { get; set; }
        public int Population => Districts.Sum(d => d.Population);
        public int EcVotes => Districts.Count + 2;
        public List<string> Regions { get; } = new List<string>();
        public List<District> Districts { get; } = new List<District>();
    }
}