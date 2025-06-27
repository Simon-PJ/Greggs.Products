using System.Collections.Generic;
using System.Linq;
using Greggs.Products.Api.Currency;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Greggs.Products.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;
    private readonly IDataAccess<Product> _productAccess;
    private readonly CurrencyConverter _currencyConverter;

    public ProductController(
        ILogger<ProductController> logger,
        IDataAccess<Product> productAccess,
        IExchangeRateService exchangeRateService)
    {
        _logger = logger;
        _productAccess = productAccess;
        _currencyConverter = new CurrencyConverter(exchangeRateService);
    }

    [HttpGet]
    public IEnumerable<EurosProduct> Get(int pageStart = 0, int pageSize = 5)
    {
        _logger.LogInformation($"Get products called with pageStart = {pageStart} and pageSize = {pageSize}");

        if (pageStart < 0 || pageSize < 0)
        {
            return Enumerable.Empty<EurosProduct>();
        }

        var products = _productAccess.List(pageStart, pageSize);

        var eurosProducts = _currencyConverter.GetProductsWithEuros(products);

        return eurosProducts;
    }
}