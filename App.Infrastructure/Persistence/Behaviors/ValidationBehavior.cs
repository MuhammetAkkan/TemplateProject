using App.Shared;
using FluentValidation;
using TS.MediatR;

namespace App.Infrastructure.Persistence.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse> // Veya senin IRequest yapın neyse
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // 1. Validator yoksa devam et
        if (!validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        // 2. Validasyonları çalıştır
        var validationFailures = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        // 3. Hataları topla
        var errors = validationFailures
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)
            .Select(validationFailure => new 
            {
                Field = validationFailure.PropertyName,
                Message = validationFailure.ErrorMessage
            })
            .Distinct()
            .ToArray();

        // 4. Hata varsa Result.Failure dön
        if (errors.Any())
        {
            // Hataları "Field: Message" formatında string array'e çevir
            var errorDetails = errors
                .Select(e => $"{e.Field}: {e.Message}")
                .ToArray();
            
            // Hata listesini Error nesnesine veriyoruz
            var error = Error.Validation("Bir veya daha fazla validasyon hatası oluştu.", errorDetails);

            // --- REFLECTION (Result veya Result<T> dönüşü için) ---
            
            var responseType = typeof(TResponse);

            // Eğer dönüş tipi Generic Result<T> ise (Örn: Result<Guid>)
            if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                var resultType = typeof(Result<>).MakeGenericType(responseType.GetGenericArguments()[0]);
                
                // Result<T>.Failure(Error error) metodunu bul
                var failureMethod = resultType.GetMethod("Failure", new[] { typeof(Error) });

                if (failureMethod is not null)
                {
                    return (TResponse)failureMethod.Invoke(null, new object[] { error })!;
                }
            }
            // Eğer dönüş tipi düz Result ise
            else if (responseType == typeof(Result))
            {
                // Result.Failure(Error error) metodunu çağır
                return (TResponse)(object)Result.Failure(error);
            }
        }

        return await next();
    }
}