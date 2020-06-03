using Primavera.Parsers.PollParsers;

namespace Primavera.Parsers.Polls.PollParsers
{
    public static class ParserFactory
    {
        public static IPollParser GetParser(int year)
        {
            IPollParser parser = null;
            switch (year)
            {
                case 2020:
                    parser = new FiveThirtyEight2020Parser();
                    break;
                case 2016:
                    parser = new FiveThirtyEight2016Parser();
                    break;
                case 2012:
                    break;
                case 2008:
                    break;
            }

            return parser;
        }
    }
}