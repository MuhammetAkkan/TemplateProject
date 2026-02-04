namespace App.Application.Interfaces.Data;

public interface ICurrentUserService
{
    Guid UserId { get; }
    Task<Guid> GetUserIdOrGuestAsync(); // DB'ye gideceği için Task
}