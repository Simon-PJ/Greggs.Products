using FluentAssertions;
using Greggs.Products.Api.Controllers;
using Greggs.Products.Api.Currency;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace Greggs.Products.IntegrationTests
{
    // I've created this because in reality product access would be calling an api or a database etc. With the unit tests,
    // the functionality in that class is hidden. We can test for real via an integration test.
    public class ProductIntegrationTests
    {
        Mock<ILogger<ProductController>> logger = new Mock<ILogger<ProductController>>();

        [Fact]
        public void TestFirstPage()
        {
            var productsAccess = new ProductAccess();
            var controller = new ProductController(logger.Object, productsAccess, new ExchangeRateService());

            var results = controller.Get(0, 3);

            results.Should().BeEquivalentTo(new List<Product> {
                new() { Name = "Sausage Roll", PriceInPounds = 1m },
                new() { Name = "Vegan Sausage Roll", PriceInPounds = 1.1m },
                new() { Name = "Steak Bake", PriceInPounds = 1.2m },
            });
        }

        [Fact]
        public void TestLaterPage()
        {
            var productsAccess = new ProductAccess();
            var controller = new ProductController(logger.Object, productsAccess, new ExchangeRateService());

            var results = controller.Get(3, 2);

            results.Should().BeEquivalentTo(new List<Product> {
                new() { Name = "Yum Yum", PriceInPounds = 0.7m },
                new() { Name = "Pink Jammie", PriceInPounds = 0.5m },
            });
        }

        [Fact]
        public void SelectionExceedsNumberOfProducts_ReturnWhatWeCan()
        {
            var productsAccess = new ProductAccess();
            var controller = new ProductController(logger.Object, productsAccess, new ExchangeRateService());

            var results = controller.Get(6, 2);

            results.Should().BeEquivalentTo(new List<Product> {
                new() { Name = "Bacon Sandwich", PriceInPounds = 1.95m },
                new() { Name = "Coca Cola", PriceInPounds = 1.2m }
            });
        }
    }
}