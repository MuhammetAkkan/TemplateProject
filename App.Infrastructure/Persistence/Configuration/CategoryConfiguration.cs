using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Persistence.Configuration;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // table name
        builder.ToTable("categories");
        
        // index
        builder.HasKey(x => x.Id);
        
        // name
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        // description
        builder.Property(x => x.Description)
            .HasMaxLength(500);
        
        // relationships
        builder.HasMany(x => x.Products)
            .WithOne(x => x.Category)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict); // bir category silindiğinde ilişkili ürünler de silinsin
        
        
    }
}