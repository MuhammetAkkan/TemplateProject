using App.Domain.Entities;
using App.Domain.Repositories;
using App.Infrastructure.Persistence.Context;

namespace App.Infrastructure.Persistence.Repositories;

public class CategoryRepository(AppDbContext context) : GenericRepository<Category>(context), ICategoryRepository;