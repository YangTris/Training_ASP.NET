using Core.Entities;
using Core.IRepositories;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task DeleteAsync(Product product)
        {
            product.IsDeleted = true;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedResult<Product>> GetProductsAsync(PaginatedFilterParams filterParams)
        {
            IQueryable<Product> query = _context.Products
                .Include(p => p.Images);


            if (!string.IsNullOrWhiteSpace(filterParams.SearchTerm))
            {
                var search = filterParams.SearchTerm.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(search));
            }

            query = filterParams.SortBy?.ToLower() switch
            {
                "name" => filterParams.IsDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                "price" => filterParams.IsDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                _ => filterParams.IsDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
            };

            var total = await query.CountAsync();
            var items = await query
                .Skip((filterParams.PageNumber - 1) * filterParams.PageSize)
                .Take(filterParams.PageSize)
                .ToListAsync();

            return new PaginatedResult<Product>
            {
                Items = items,
                TotalItems = total,
                PageNumber = filterParams.PageNumber,
                PageSize = filterParams.PageSize
            };
        }

        public async Task<Product?> GetByIdAsync(Guid productId)
        {
            return await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
    }
}
