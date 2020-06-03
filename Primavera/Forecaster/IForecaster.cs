using System;
using Primavera.Results;

namespace Primavera.Forecaster
{
    public interface IForecaster
    {
        ForecastResult Forecast(DateTime forecastDate);
    }
}