using App.Application.Features.Categories.Commands.Create;
using App.Application.Features.Categories.Commands.Delete;
using App.Application.Features.Categories.Commands.Update;
using App.Application.Features.Categories.Queries.GetById;
using App.Application.Features.Categories.Queries.GetCategoriesPagedList;
using App.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TS.MediatR;

namespace App.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize(AuthenticationSchemes = "Bearer")]
public class CategoryController(ISender sender) : ApiController(sender)
{
    [HttpPost("create")]
    // success: 201
    [ProducesResponseType(typeof(Result<CreateCategoryResponse>), StatusCodes.Status201Created)]
    // failure: 400
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command, CancellationToken ct)
    {
        var result = await Sender.Send(command, ct);
        
        return ResultResponse(result);
    }
    
    // update
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdateCategoryCommand command, CancellationToken ct)
    {
        var result = await Sender.Send(command, ct);

        return ResultResponse(result);
    }
    
    // delete
    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var command = new DeleteCategoryCommand(id);
        var result = await Sender.Send(command, ct);

        return ResultResponse(result);
    }
    
    //[Authorize(AuthenticationSchemes =  "Bearer")]
    [HttpGet("get/{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var query = new GetCategoryByIdQuery(id);
        var result = await Sender.Send(query, ct);
        return ResultResponse(result);
    }
    
    // get all paged
    [HttpGet("get-paged")]
    public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var query = new GetCategoriesPagedListQuery(pageNumber, pageSize);
        var result = await Sender.Send(query, ct);
        return ResultResponse(result);
    }
}