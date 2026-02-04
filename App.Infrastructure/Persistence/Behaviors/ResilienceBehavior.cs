using System.Reflection;
using App.Application.Abstractions.Behaviors;
using Microsoft.Extensions.Logging;
using Polly;
using TS.MediatR;

namespace App.Infrastructure.Persistence.Behaviors;

public sealed class ResilienceBehavior<TRequest, TResponse>(ILogger<ResilienceBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // 1. İstek üzerinde [RetryPolicy] attribute'u var mı kontrol et
        var retryAttribute = request.GetType().GetCustomAttribute<RetryPolicyAttribute>();

        // 2. Attribute yoksa pipeline'a normal devam et (Polly devreye girmez)
        if (retryAttribute is null)
        {
            return await next();
        }

        // 3. Polly Politikasını Oluştur
        var retryPolicy = Policy
            .Handle<Exception>(ex => 
            {
                // Burada hangi hataların tekrar deneneceğini seçebilirsin.
                // Örn: Veritabanı hatalarında dene ama Validation hatası fırlatılmışsa deneme.
                // Şimdilik genel Exception tutuyoruz.
                return true; 
            })
            .WaitAndRetryAsync(
                retryCount: retryAttribute.RetryCount,
                sleepDurationProvider: retryAttempt => TimeSpan.FromMilliseconds(retryAttribute.SleepDurationMilliseconds),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    // Her denemede log atıyoruz (Serilog bunu yakalar)
                    logger.LogWarning(
                        "Resilience: '{RequestName}' işleminde hata oluştu: {Message}. {RetryCount}. kez tekrar deneniyor...",
                        typeof(TRequest).Name,
                        exception.Message,
                        retryCount);
                }
            );

        // 4. İsteği Polly koruması (Wrapper) altında çalıştır
        // Eğer tüm denemeler başarısız olursa, Polly son hatayı fırlatır (Throw).
        // Bu hatayı 'GlobalExceptionHandler' yakalar.
        return await retryPolicy.ExecuteAsync(async () => await next());
    }
}