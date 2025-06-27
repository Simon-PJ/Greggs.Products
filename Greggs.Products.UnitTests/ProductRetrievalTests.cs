using FluentAssertions;
using Greggs.Products.Api.Controllers;
using Greggs.Products.Api.Currency;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Greggs.Products.UnitTests;

public class ProductRetrievalTests
{
    Mock<ILogger<ProductController>> logger = new Mock<ILogger<ProductController>>();
    Mock<IDataAccess<Product>> productAccess = new Mock<IDataAccess<Product>>();
    Mock<IExchangeRateService> exchangeRateService = new Mock<IExchangeRateService>();

    [Fact]
    public void RetrievesExpectedProducts()
    {
        var pageStart = 0;
        var pageSize = 2;

        var mockProducts = new List<Product> { new Product { Name = "Product 1" }, new Product { Name = "Product2" } };

        productAccess.Setup(x => x.List(pageStart, pageSize)).Returns(mockProducts);

        var controller = new ProductController(logger.Object, productAccess.Object, exchangeRateService.Object);

        var result = controller.Get(pageStart, pageSize);

        result.Should().BeEquivalentTo(mockProducts);
    }

    [Fact]
    public void CreatesALogEntry()
    {
        var controller = new ProductController(logger.Object, productAccess.Object, exchangeRateService.Object);

        controller.Get(2, 4);

        // Only checking for a log call of the correct level, not the content. Wouldn't want to fail tests if I tweaked the message
        logger.Verify(
        x => x.Log(
            It.Is<LogLevel>(l => l == LogLevel.Information),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
    }

    // Maybe would prefer to return a bad status code. I've gone with empty products for now
    [Theory]
    [InlineData(-1, 3)]
    [InlineData(1, -2)]
    public void NegativePageInfo_ReturnEmptyProducts(int pageStart, int pageSize)
    {
        var mockProducts = new List<Product> { new Product { Name = "Product 1" }, new Product { Name = "Product2" } };

        productAccess.Setup(x => x.List(It.IsAny<int>(), It.IsAny<int>())).Returns(mockProducts);

        var controller = new ProductController(logger.Object, productAccess.Object, exchangeRateService.Object);

        var result = controller.Get(pageStart, pageSize);

        result.Should().BeEmpty();
    }
}