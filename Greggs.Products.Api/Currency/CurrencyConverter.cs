using Greggs.Products.Api.Models;
using System.Collections.Generic;
using System.Linq;

namespace Greggs.Products.Api.Currency
{
    public class CurrencyConverter
    {
        private readonly IExchangeRateService exchangeRateService;

        public CurrencyConverter(IExchangeRateService exchangeRateService)
        {
            this.exchangeRateService = exchangeRateService;
        }

        public IEnumerable<EurosProduct> GetProductsWithEuros(IEnumerable<Product> products)
        {
            var exchangeRate = exchangeRateService.GbpToEuro();

            return products.Select(product =>
            {
                var priceInEuros = product.PriceInPounds * exchangeRate;

                var euroProduct = new EurosProduct
                {
                    Name = product.Name,
                    PriceInPounds = product.PriceInPounds,
                    PriceInEuros = priceInEuros
                };

                return euroProduct;
            });
        }
    }
}
