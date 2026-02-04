using App.Domain.Entities;
using App.Domain.Repositories;
using App.Infrastructure.Persistence.Context;

namespace App.Infrastructure.Persistence.Repositories;

public class ProductRepository(AppDbContext context) : GenericRepository<Product>(context), IProductRepository
{
    
}