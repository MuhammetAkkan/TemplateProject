using System.Net;
using App.Shared;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace App.Infrastructure.Persistence.Errors;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) 
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        // 1. Hatayı logla (Sistem hatası olduğu için detaylı log önemli)
        logger.LogError(exception, "Beklenmedik bir hata oluştu: {Message}", exception.Message);

        // 2. Result formatında hata nesnesi oluştur
        var error = Error.InternalServer("Sunucu tarafında beklenmedik bir hata oluştu.");
        var result = Result.Failure(error);

        // 3. Response ayarlarını yap
        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        httpContext.Response.ContentType = "application/json";

        // 4. JSON olarak geri dön
        await httpContext.Response.WriteAsJsonAsync(result, cancellationToken);

        return true; // Hatanın ele alındığını sisteme bildirir
    }
}