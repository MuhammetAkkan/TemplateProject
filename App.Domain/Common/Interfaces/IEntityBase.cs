namespace App.Domain.Common.Interfaces;

public interface IEntityBase
{
    Guid Id { get; set; }
    DateTimeOffset CreatedAt { get; set; }
    DateTimeOffset? UpdatedAt { get; set; }
}