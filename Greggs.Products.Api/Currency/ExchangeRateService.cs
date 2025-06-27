namespace Greggs.Products.Api.Currency
{
    // In reality we'd hit an api for this live, or have a database entry that is populated each day by a daily
    // running service.
    public class ExchangeRateService : IExchangeRateService
    {
        private const decimal gbpToEuroExchangeRate = 1.11m;

        // We could have a general endpoint that converts to many currencies. But this would introduce complexity
        // that we don't have to worry about while the requirements are only euro conversions
        public decimal GbpToEuro() => gbpToEuroExchangeRate;
    }
}
