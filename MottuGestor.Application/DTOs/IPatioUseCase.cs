using MottuGestor.Domain.Entities;
using MottuGestor.Domain.Pagination;

namespace MottuGestor.Application.DTOs;

public interface IPatioUseCase
{
    Task<PageResult<Patio>> GetPaginationAsyncUsuario(int page, int pageSize);
}