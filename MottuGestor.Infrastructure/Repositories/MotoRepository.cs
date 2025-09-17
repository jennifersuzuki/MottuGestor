using Microsoft.EntityFrameworkCore;
using MottuGestor.Domain.Entities;
using MottuGestor.Domain.Pagination;
using MottuGestor.Infrastructure.Context;

namespace MottuGestor.Infrastructure.Repositories;

public class MotoRepository : IMotoRepository
{
    private readonly GestMottuContext _context;
    public MotoRepository(GestMottuContext context) => _context = context;

    public async Task<PageResult<Moto>> GetPaginationAsyncMoto(
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var query = _context.Motos
            .AsNoTracking()
            .OrderByDescending(n => n.DataCadastro);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PageResult<Moto>
        {
            Items = items,
            Total = total,
            HasMore = page * pageSize < total,
            Page = page,
            PageSize = pageSize
        };
    }
}