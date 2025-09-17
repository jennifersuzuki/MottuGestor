using MottuGestor.Domain.Entities;
using MottuGestor.Domain.Pagination;

namespace MottuGestor.Application.DTOs;

public interface IMotoUseCase
{
    Task<PageResult<Moto>> GetPaginationAsyncMoto(int page, int pageSize);
}