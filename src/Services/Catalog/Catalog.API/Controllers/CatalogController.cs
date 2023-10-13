    using System.Net;
using Catalog.API.Entities;
using Catalog.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly ILogger<CatalogController> _logger;
    private readonly IProductRepository _productRepository;

    public CatalogController(ILogger<CatalogController> logger, IProductRepository productRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), (int) HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        var products = await _productRepository.GetProducts();
        return Ok(products);
    }

    [HttpGet("{id:length(24)}", Name = "GetProduct")]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Product), (int) HttpStatusCode.OK)]
    public async Task<ActionResult<Product>> GetProductById(string id)
    {
        var product = await _productRepository.GetProduct(id);
        if (product is null)
        {
            _logger.LogError($"{nameof(Product)} with id: {id}, not found.");
            return NotFound();
        }

        return Ok(product);
    }

    [HttpGet]
    [Route("[action]/{category}", Name = "GetProductByCategory")]
    [ProducesResponseType(typeof(IEnumerable<Product>), (int) HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string category)
    {
        var products = await _productRepository.GetProductByCategory(category);
        return Ok(products);
    }

    [HttpGet]
    [Route("[action]/{name}", Name = "GetProductByName")]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IEnumerable<Product>), (int) HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductByName(string name)
    {
        var items = await _productRepository.GetProductByName(name);
        if (items == null)
        {
            _logger.LogError($"Products with name: {name} not found.");
            return NotFound();
        }

        return Ok(items);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Product), (int) HttpStatusCode.OK)]
    public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
    {
        await _productRepository.CreateProduct(product);
        return CreatedAtRoute("GetProduct", new {id = product.Id}, product);
    }

    [HttpPut]
    [ProducesResponseType(typeof(Product), (int) HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateProduct([FromBody] Product product)
    {
        return Ok(await _productRepository.UpdateProduct(product));
    }

    [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
    [ProducesResponseType(typeof(Product), (int) HttpStatusCode.OK)]
    public async Task<IActionResult> DeleteProductById(string id)
    {
        return Ok(await _productRepository.DeleteProduct(id));
    }
}