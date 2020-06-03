using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Primavera.Data;
using Primavera.Parsers.Util;

namespace Primavera.Parsers.States.StateParsers
{
    public class CensusBureauStateParser : IStateParser
    {
        private const string StateDataEndpoint =
            "https://www.census.gov/mycd/application/bin/functs_easystats.local.php?call=get_geography&geo_level_1=*&geo_level_2=*&url=https%3A%2F%2Fapi.census.gov%2Fdata%2F2018%2Facs%2Facs1%2Fprofile&key=f4a93d15173229253a4f234727b2902053f61bbd%3Bmycd&geo_type=STATE";

        private const string DistrictDataEndpoint =
            "https://www.census.gov/mycd/application/bin/functs_easystats.local.php?call=get_geography&geo_type=CONGRESSIONAL_DISTRICT&geo_level_1={0}&geo_level_2=*&url=https%3A%2F%2Fapi.census.gov%2Fdata%2F2018%2Facs%2Facs1%2Fprofile&key=f4a93d15173229253a4f234727b2902053f61bbd%3Bmycd";

        private const string DistrictDetailsEndpoint =
            "https://www.census.gov/mycd/application/bin/functs_easystats.php?call=get_values&geo_type=CONGRESSIONAL_DISTRICT&geo_level_1={0}&geo_level_2={1}&url=https%3A%2F%2Fapi.census.gov%2Fdata%2F2018%2Facs%2Facs1%2Fprofile&tableid=99_mcd_people&key=f4a93d15173229253a4f234727b2902053f61bbd%3Bmycd";

        public async Task<State[]> GetStatesAsync()
        {
            using var httpClient = new HttpClient();

            var states = new List<State>();

            RegionData[] allStates =
                await HttpHelper.GetAsync<RegionData[]>(new Uri(StateDataEndpoint)).ConfigureAwait(false);

            foreach (RegionData state in allStates)
            {
                if (Constants.States.Any(s => s.Name == state.Name))
                {
                    var districts = new List<District>();

                    RegionData[] stateDistricts = await HttpHelper
                        .GetAsync<RegionData[]>(new Uri(string.Format(CultureInfo.InvariantCulture,
                            DistrictDataEndpoint, state.Fips))).ConfigureAwait(false);

                    List<Task<DistrictData>> districtTasks = stateDistricts
                        .Select(district =>
                            HttpHelper.GetAsync<DistrictData>(new Uri(string.Format(CultureInfo.InvariantCulture,
                                DistrictDetailsEndpoint, state.Fips, district.Fips))))
                        .ToList();

                    foreach (Task<DistrictData> task in districtTasks)
                    {
                        DistrictData data = await task.ConfigureAwait(false);
                        districts.Add(new District
                        {
                            Name = data.Name,
                            Population = data.Population
                        });
                    }

                    var result = new State
                    {
                        Name = state.Name,
                        Abbreviation = Constants.States.FirstOrDefault(s => s.Name == state.Name).Abbreviation
                    };

                    result.Districts.AddRange(districts);

                    states.Add(result);
                }
            }

            return states.ToArray();
        }

        private class RegionData
        {
            [JsonProperty("name")] public string Name { get; set; }

            [JsonProperty("fips")] public string Fips { get; set; }
        }

        private class DistrictData
        {
            [JsonProperty("NAME")] public string Name { get; set; }

            [JsonProperty("DP05_0001E")] public int Population { get; set; }
        }
    }
}