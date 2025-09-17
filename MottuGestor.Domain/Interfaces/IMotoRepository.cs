using MottuGestor.Domain.Entities;
using MottuGestor.Domain.Pagination;

namespace MottuGestor.Infrastructure.Repositories;

public interface IMotoRepository
{
    Task<PageResult<Moto>> GetPaginationAsyncMoto(int page, int pageSize, CancellationToken cancellationToken = default);
}