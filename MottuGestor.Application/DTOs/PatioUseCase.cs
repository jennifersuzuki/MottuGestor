using MottuGestor.Domain.Entities;
using MottuGestor.Domain.Pagination;
using MottuGestor.Infrastructure.Repositories;

namespace MottuGestor.Application.DTOs;

public class PatioUseCase : IPatioUseCase
{
    private readonly IPatioRepository _repository;

    public PatioUseCase(IPatioRepository repository)
        => _repository = repository;

    public Task<PageResult<Patio>> GetPaginationAsyncUsuario(int page, int pageSize)
        => _repository.GetPaginationAsyncPatio(page, pageSize);
}