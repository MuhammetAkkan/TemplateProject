using App.Domain.Common.Interfaces;

namespace App.Domain.Entities;

public sealed class ErrorLog : BaseEntity
{
    public string ErrorCode { get; set; }
    public string Message { get; set; }
    public string RequestName { get; set; }
    public string RequestData { get; set; } // Hata anındaki parametreler
}