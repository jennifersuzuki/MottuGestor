using MottuGestor.Domain.Entities;
using MottuGestor.Domain.Pagination;

namespace MottuGestor.Infrastructure.Repositories;

public interface IUsuarioRepository
{
    Task<PageResult<Usuario>> GetPaginationAsyncUsuario(int page, int pageSize, CancellationToken cancellationToken = default);
}