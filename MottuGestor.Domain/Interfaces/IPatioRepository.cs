using MottuGestor.Domain.Entities;
using MottuGestor.Domain.Pagination;

namespace MottuGestor.Infrastructure.Repositories;

public interface IPatioRepository
{
    Task<PageResult<Patio>> GetPaginationAsyncPatio(int page, int pageSize, CancellationToken cancellationToken = default);
}