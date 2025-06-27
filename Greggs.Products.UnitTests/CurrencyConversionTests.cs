using FluentAssertions;
using Greggs.Products.Api.Controllers;
using Greggs.Products.Api.Currency;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Greggs.Products.UnitTests
{
    public class CurrencyConversionTests
    {
        Mock<ILogger<ProductController>> logger = new Mock<ILogger<ProductController>>();
        Mock<IDataAccess<Product>> productAccess = new Mock<IDataAccess<Product>>();
        Mock<IExchangeRateService> exchangeRateService = new Mock<IExchangeRateService>();

        [Fact]
        public void ProductsHaveTheCorrectEuroAmount()
        {
            var pageStart = 0;
            var pageSize = 2;

            exchangeRateService.Setup(x => x.GbpToEuro()).Returns(2);

            var mockProducts = new List<Product> { new Product { Name = "Product 1", PriceInPounds = 1m }, new Product { Name = "Product 2", PriceInPounds = 5.67m } };

            productAccess.Setup(x => x.List(pageStart, pageSize)).Returns(mockProducts);

            var controller = new ProductController(logger.Object, productAccess.Object, exchangeRateService.Object);

            var result = controller.Get(pageStart, pageSize);

            result.Count().Should().Be(2);
            result.Single(x => x.Name == "Product 1").PriceInEuros.Should().Be(2m);
            result.Single(x => x.Name == "Product 2").PriceInEuros.Should().Be(11.34m);
        }
    }
}
