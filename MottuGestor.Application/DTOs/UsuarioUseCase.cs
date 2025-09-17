using MottuGestor.Domain.Entities;
using MottuGestor.Domain.Pagination;
using MottuGestor.Infrastructure.Repositories;

namespace MottuGestor.Application.DTOs;

public class UsuarioUseCase : IUsuarioUseCase
{
    private readonly IUsuarioRepository _repository;

    public UsuarioUseCase(IUsuarioRepository repository)
        => _repository = repository;

    public Task<PageResult<Usuario>> GetPaginationAsyncUsuario(int page, int pageSize)
        => _repository.GetPaginationAsyncUsuario(page, pageSize);
}