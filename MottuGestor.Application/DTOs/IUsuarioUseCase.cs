using MottuGestor.Domain.Entities;
using MottuGestor.Domain.Pagination;

namespace MottuGestor.Application.DTOs;

public interface IUsuarioUseCase
{
    Task<PageResult<Usuario>> GetPaginationAsyncUsuario(int page, int pageSize);
}