using Microsoft.AspNetCore.Mvc;
using testing.Services.Interfaces;
using testing.Response;
using testing.Request;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly IProductService _service;

    public ProductController(IProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<GetAllProductResponse>> GetAll()
    {
        return this.Ok(await _service.GetAllProducts());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetProductByIdResponse>> GetById(int id)
    {
       

        var product = await _service.GetProductById(id);
        if (product == null)
            return NotFound();

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult> Add(AddProductResquest product)
    {
        var result = await _service.AddProduct(product);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateProductResquest product)
    {
        var result = await _service.UpdateProduct(id, product);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _service.DeleteProduct(id);
        return Ok(result);
    }
}