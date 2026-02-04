using App.Application.Abstractions.Cache;
using App.Application.Abstractions.Services;
using App.Application.Features.Categories.Dto;
using App.Application.Interfaces.Repositories;
using App.Shared;
using TS.MediatR;

namespace App.Application.Features.Categories.Queries.GetById;

public class GetCategoryByIdQueryHandler(
    ICategoryQueryRepository queryRepository,
    ICacheService cacheService,
    IJobService jobService) // 1. Inject ettik
    : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
{
    public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken ct)
    {
        // 2. Cache Key Oluşturma
        string key = $"categories:detail:{request.Id}";

        // 3. Cache Kontrolü (DTO tipinde istiyoruz)
        var cachedCategory = await cacheService.GetAsync<CategoryDto>(key, ct);

        if (cachedCategory is not null) return cachedCategory;


        // 4. Veritabanından Veri Çekme
        var category = await queryRepository.GetByIdAsync(request.Id);

        if (category is null)
        {
            return Error.NotFound("Kategori bulunamadı.");
        }

        // 5. Cache'e Yazma
        await cacheService.SetAsync(key, category, TimeSpan.FromMinutes(30), ct);
        
        // hangfire job ekleme => deneme için
        jobService.Enqueue(() =>
            Console.WriteLine($"----> HANGFIRE JOB: Kategori ({category.Name}) görüntülendi! Rapor hazırlanıyor...")
        );

        return category;
    }
    // polly ile denemek için yorumdan çıkarabilirsiniz
    /*
    private static int _requestCounter = 0;
    public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken ct)
    {
        _requestCounter++;

        // İLK 2 DENEMEDE HATA FIRLAT (Veritabanı kopmuş gibi yapıyoruz)
        if (_requestCounter <= 2)
        {
            // Bu hata ResilienceBehavior tarafından yakalanacak ve Loglanacak
            throw new Exception("Veritabanı bağlantısı koptu! (Simülasyon)");
        }

        // 3. DENEMEDE (veya sonrasında) BAŞARILI OLACAK
        // ... Normal kodların buradan devam ediyor ...
        string key = $"categories:detail:{request.Id}";

        // ... cache ve repo kodların ...
        var category = await queryRepository.GetByIdAsync(request.Id);

        // Test bittikten sonra sayacı sıfırlayalım ki tekrar test edebilesin
        _requestCounter = 0;

        if (category is null) return Error.NotFound("Kategori bulunamadı.");

        return category;
    }
    */
}