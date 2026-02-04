using Hangfire.Dashboard;

namespace App.Presentation.Extensions;

/// <summary>
/// Hangfire Dashboard erişim kontrolü
/// PRODUCTION ortamında burayı JWT ile korumalısınız!
/// </summary>
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // Development ortamında herkese açık
        // Production'da burada JWT kontrolü yapılmalı veya IP bazlı kısıtlama konulmalı
        return true;
    }
}
