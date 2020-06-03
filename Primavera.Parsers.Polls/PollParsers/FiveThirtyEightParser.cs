using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Primavera.Data;
using Primavera.Parsers.PollParsers;

namespace Primavera.Parsers.Polls.PollParsers
{
    public abstract class FiveThirtyEightParser : IPollParser
    {
        protected abstract Uri Url { get; }

        public async Task<Poll[]> GetPollsAsync()
        {
            using var client = new HttpClient();
            HttpResponseMessage result = await client.GetAsync(Url).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                await using Stream stream = await result.Content.ReadAsStreamAsync().ConfigureAwait(false);
                return ParseFromStream(stream);
            }

            return Array.Empty<Poll>();
        }

        protected abstract Poll[] ParseFromStream(Stream stream);
    }
}