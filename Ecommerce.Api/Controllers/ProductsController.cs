using Ecommerce.Application.Contracts.Pagination;
using Ecommerce.Application.Contracts.Products;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ProductsController(IProductService productService) : ControllerBase
{
    private readonly IProductService productService = productService;
    [HttpGet("")]
    [Authorize(Roles = DefaultRoles.Member.Name)]
    public async Task<IActionResult> GetProducts([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        var products = await productService.GetProductsPagedAsync(filters, cancellationToken);
        return Ok(products.Value);
    }
    [HttpGet("{id}")]
    [Authorize(Roles = DefaultRoles.Member.Name)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await productService.GetByIdAsync(id);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }
    [HttpPost("")]
    [Authorize(Roles = DefaultRoles.Admin.Name)]
    public async Task<IActionResult> Create([FromBody] ProductRequest request,
        [FromServices] IValidator<ProductRequest> validator, CancellationToken cancellationToken)
    {
        var ValidatorResult = await validator.ValidateAsync(request, cancellationToken);

        if (!ValidatorResult.IsValid)
            return ValidatorResult.ToProblem();

        var result = await productService.CreateAsync(request, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id },result.Value)
            : result.ToProblem();
    }
    [HttpPut("{Id}")]
    [Authorize(Roles = DefaultRoles.Admin.Name)]
    public async Task<IActionResult> Update(int Id, [FromBody] ProductRequest request,
        [FromServices] IValidator<ProductRequest> validator, CancellationToken cancellationToken)
    {
        var ValidatorResult = await validator.ValidateAsync(request, cancellationToken);

        if (!ValidatorResult.IsValid)
            return ValidatorResult.ToProblem();

        var result = await productService.UpdateAsync(Id, request, cancellationToken);
        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
    [HttpPut("{id}/toggle-status")]
    [Authorize(Roles = DefaultRoles.Admin.Name)]
    public async Task<IActionResult> ToggleStatus([FromRoute]int id, CancellationToken cancellationToken)
    {
        var result = await productService.TogglePublishStatusAsync(id, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();

    }
}
