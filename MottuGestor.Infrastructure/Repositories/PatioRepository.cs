using Microsoft.EntityFrameworkCore;
using MottuGestor.Domain.Entities;
using MottuGestor.Domain.Pagination;
using MottuGestor.Infrastructure.Context;

namespace MottuGestor.Infrastructure.Repositories;

public class PatioRepository : IPatioRepository
{
    private readonly GestMottuContext _context;
    public PatioRepository(GestMottuContext context) => _context = context;

    public async Task<PageResult<Patio>> GetPaginationAsyncPatio(
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var query = _context.Patios
            .AsNoTracking()
            .OrderByDescending(n => n.Nome);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PageResult<Patio>
        {
            Items = items,
            Total = total,
            HasMore = page * pageSize < total,
            Page = page,
            PageSize = pageSize
        };
    }
}