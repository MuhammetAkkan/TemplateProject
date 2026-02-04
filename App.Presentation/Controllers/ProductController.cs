using App.Application.Features.Products.Commands.Create;
using App.Application.Features.Products.Queries.GetProductsPagedList;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TS.MediatR;

namespace App.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController(ISender sender) : ApiController(sender)
{
    // create product
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command, CancellationToken ct)
    {
        var result =  await Sender.Send(command, ct);
        return ResultResponse(result);
    }
    
    // get paged products
    [HttpGet("get-paged")]
    public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var query = new GetProductsPagedListQuery(pageNumber, pageSize);
        var result = await Sender.Send(query, ct);
        return ResultResponse(result);
    }
}