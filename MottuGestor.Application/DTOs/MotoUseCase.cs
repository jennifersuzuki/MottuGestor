using MottuGestor.Domain.Entities;
using MottuGestor.Domain.Pagination;
using MottuGestor.Infrastructure.Repositories;

namespace MottuGestor.Application.DTOs;

public class MotoUseCase : IMotoUseCase
{
    private readonly IMotoRepository _repository;

    public MotoUseCase(IMotoRepository repository)
        => _repository = repository;

    public Task<PageResult<Moto>> GetPaginationAsyncMoto(int page, int pageSize)
        => _repository.GetPaginationAsyncMoto(page, pageSize);
}