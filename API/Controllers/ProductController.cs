using Application.DTOs.Product;
using Application.IServices;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductListDTO>>> GetAll([FromQuery] PaginatedFilterParams filterParams)
        {
            var products = await _productService.GetAllProductsAsync(filterParams);
            return Ok(products);
        }

        [HttpGet("{productId}")]
        public async Task<ActionResult<ProductDetailDTO>> GetById(Guid productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDetailDTO>> Create([FromBody] CreateProductDTO createProductDTO)
        {
            var created = await _productService.CreateProductAsync(createProductDTO);
            return CreatedAtAction(nameof(GetById), new { productId = created.Id }, created);
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> Update(Guid productId, [FromBody] UpdateProductDTO updateProductDTO)
        {
            await _productService.UpdateProductAsync(productId, updateProductDTO);
            return NoContent();
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete(Guid productId)
        {
            await _productService.DeleteProductAsync(productId);
            return NoContent();
        }
    }
}
