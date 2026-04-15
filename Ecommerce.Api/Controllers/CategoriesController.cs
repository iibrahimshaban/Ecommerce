using Ecommerce.Application.Contracts.Categories;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    private readonly ICategoryService _categoryService = categoryService;
    [HttpGet("")]
    [Authorize(Roles = DefaultRoles.Admin.Name)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetAllAsync(cancellationToken);
        return Ok(categories);
    }
    [HttpGet("{id}")]
    [Authorize(Roles = DefaultRoles.Member.Name)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }
    [HttpPost("")]
    [Authorize(Roles = DefaultRoles.Admin.Name)]
    public async Task<IActionResult> Create([FromBody] CategoryRequest request,
        [FromServices] IValidator<CategoryRequest> validator, CancellationToken cancellationToken)
    {
        var ValidatorResult = await validator.ValidateAsync(request, cancellationToken);

        if (!ValidatorResult.IsValid)
            return ValidatorResult.ToProblem();


        var result = await _categoryService.CreateAsync(request, cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
            : result.ToProblem();
    }
    [HttpPut("{Id}")]
    [Authorize(Roles = DefaultRoles.Admin.Name)]
    public async Task<IActionResult> Update([FromRoute]int Id, [FromBody] CategoryRequest request,
        [FromServices] IValidator<CategoryRequest> validator, CancellationToken cancellationToken)
    {
        var ValidatorResult = await validator.ValidateAsync(request, cancellationToken);

        if (!ValidatorResult.IsValid)
            return ValidatorResult.ToProblem();

        var result = await _categoryService.UpdateAsync(Id, request, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

}
