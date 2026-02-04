using System.Text.Json;
using App.Domain.Entities;
using App.Infrastructure.Persistence.Context;
using App.Shared;
using Microsoft.Extensions.Logging;
using TS.MediatR;

namespace App.Infrastructure.Persistence.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger,
    AppDbContext context) // Logları kaydetmek istediğin DbContext
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
    where TResponse : Result // Sadece Result veya Result<T> dönenleri yakalar
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // 1. İşlemi çalıştır ve sonucu al
        var response = await next();

        // 2. Eğer sonuç başarısızsa loglama işlemlerini yap
        if (response.IsFailure)
        {
            var error = response.Error;
            var requestName = typeof(TRequest).Name;
            var requestData = JsonSerializer.Serialize(request);

            // Konsola detaylı hata logu bas
            logger.LogError("Hata Oluştu! Request: {RequestName} | Kod: {Code} | Mesaj: {Message}", 
                requestName, error?.Code, error?.Message);

            // Veritabanına hata logunu kaydet
            try 
            {
                var errorLog = new ErrorLog
                {
                    Id = Guid.NewGuid(),
                    RequestName = requestName,
                    RequestData = requestData,
                    ErrorCode = error?.Code ?? "Unknown",
                    Message = error?.Message ?? "Bilinmeyen hata",
                    CreatedAt = DateTime.UtcNow
                };
                // Eğer ErrorLog diye bir entity'n varsa:
                await context.Set<ErrorLog>().AddAsync(errorLog);
                await context.SaveChangesAsync(cancellationToken);
                
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Hata logu veritabanına kaydedilemedi!");
            }
        }

        return response;
    }
}