using System;

namespace Primavera.Math
{
    public static class Guassian
    {
        private static readonly Random _random = new Random();

        public static decimal Next(decimal mean, decimal stdDev)
        {
            double u1 = 1.0 - _random.NextDouble();
            double u2 = 1.0 - _random.NextDouble();
            double randStdNormal = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) *
                                   System.Math.Sin(2.0 * System.Math.PI * u2);
            decimal randNormal = mean + stdDev * (decimal) randStdNormal;
            return randNormal;
        }
    }
}