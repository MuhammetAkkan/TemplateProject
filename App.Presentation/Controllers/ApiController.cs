using App.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TS.MediatR;

namespace App.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiController(ISender sender) : ControllerBase
{
    protected readonly ISender Sender = sender;

    /// <summary>
    /// Genel işlemler için (GET, PUT vb.)
    /// </summary>
    [NonAction]
    protected IActionResult ResultResponse<T>(Result<T> result) =>
        result.IsSuccess ? Ok(result) : HandleError(result);

    /// <summary>
    /// CREATE işlemleri için (POST)
    /// </summary>
    [NonAction]
    protected IActionResult CreatedResponse<T>(Result<T> result) =>
        result.IsSuccess ? StatusCode(StatusCodes.Status201Created, result) : HandleError(result);
    
    /// <summary>
    /// Data dönmeyen işlemler için (Update, Delete)
    /// </summary>
    [NonAction]
    protected IActionResult ResultResponse(Result result) =>
        result.IsSuccess ? Ok(result) : HandleError(result);

    // Merkezi Hata Yönetimi
    private IActionResult HandleError(Result result)
    {
        // Error null ise fail-safe olarak 500 döner.
        if (result.Error is null)
            return StatusCode(500, "Bilinmeyen bir hata oluştu.");

        return result.Error.Type switch
        {
            ErrorType.Validation => BadRequest(result),
            ErrorType.NotFound   => NotFound(result),
            ErrorType.Conflict   => Conflict(result),
            _                    => BadRequest(result) 
        };
    }
}