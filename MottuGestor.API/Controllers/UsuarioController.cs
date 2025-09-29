using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuGestor.API.Models;
using MottuGestor.Domain.Entities;
using MottuGestor.Domain.Pagination;
using MottuGestor.Infrastructure.Context;
using MottuGestor.Infrastructure.Repositories;

namespace MottuGestor.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IRepository<Usuario> _usuarioRepository;
        private readonly LinkGenerator _links;

        public UsuarioController(IRepository<Usuario> usuarioRepository, LinkGenerator links)
        {
            _usuarioRepository = usuarioRepository;
            _links = links;
        }

        // ============================
        // [GET] /usuario
        // Retorna todos os usuários cadastrados
        // ============================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            return Ok(usuarios);
        }

        // ============================
        // [GET] /usuario/{id}
        // Busca um usuário específico por ID
        // ============================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            return Ok(usuario);
        }

        // ============================
        // [GET] /usuario/filtro
        // Filtra usuários por nome e email (via query string)
        // Exemplo: /usuario/filtro?nome=joao&email=teste
        // ============================
        [HttpGet("filtro")]
        public async Task<IActionResult> Filtrar(
            [FromQuery] string? nome,
            [FromQuery] string? email)
        {
            var usuarios = await _usuarioRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(nome))
                usuarios = usuarios.Where(u => u.Nome.ToLower().Contains(nome.ToLower())).ToList();

            if (!string.IsNullOrWhiteSpace(email))
                usuarios = usuarios.Where(u => u.Email.ToLower().Contains(email.ToLower())).ToList();

            var resultado = usuarios.Select(u => new
            {
                u.UsuarioId,
                u.Nome,
                u.Email,
                u.DataCadastro
            });

            return Ok(resultado);
        }

        // ============================
        // [POST] /usuario
        // Cria um novo usuário
        // ============================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UsuarioInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var usuario = new Usuario(
                    nome: input.Nome,
                    email: input.Email,
                    senhaHash: input.SenhaHash
                );

                await _usuarioRepository.AddAsync(usuario);
                await _usuarioRepository.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = usuario.UsuarioId }, usuario);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao cadastrar usuário: {ex.Message}");
            }
        }

        // ============================
        // [PUT] /usuario/{id}
        // Atualiza os dados de um usuário existente
        // ============================
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UsuarioInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            try
            {
                usuario.AtualizarNome(input.Nome);
                usuario.AtualizarEmail(input.Email);
                usuario.AtualizarSenha(input.SenhaHash);

                await _usuarioRepository.UpdateAsync(usuario);
                await _usuarioRepository.SaveChangesAsync();

                var response = new Usuario.UsuarioResponse(
                    usuario.UsuarioId, usuario.Nome, usuario.Email
                );

                return Ok(response);
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar usuário: {ex.Message}");
            }
        }

        // ============================
        // [DELETE] /usuario/{id}
        // Remove um usuário pelo ID
        // ============================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            try
            {
                await _usuarioRepository.DeleteAsync(id);
                await _usuarioRepository.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao excluir usuário: {ex.Message}");
            }
        }
        
        [HttpGet("paginado", Name = "GetUsuariosPaged")]                  // <-- nome da rota
        [Produces("application/hal+json")]                                 // <-- HAL
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = "Nome",
            [FromQuery] string? sortDir = "Asc",
            CancellationToken ct = default
        )
        {
            var all = await _usuarioRepository.GetAllAsync();
            var q = all.AsQueryable();

            // filtro simples
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(u => u.Nome.ToLower().Contains(s) || u.Email.ToLower().Contains(s));
            }

            // ordenação
            var asc = string.Equals(sortDir, "Asc", StringComparison.OrdinalIgnoreCase);
            q = (sortBy?.ToLowerInvariant()) switch
            {
                "email"        => asc ? q.OrderBy(u => u.Email)        : q.OrderByDescending(u => u.Email),
                "datacadastro" => asc ? q.OrderBy(u => u.DataCadastro) : q.OrderByDescending(u => u.DataCadastro),
                _              => asc ? q.OrderBy(u => u.Nome)         : q.OrderByDescending(u => u.Nome),
            };

            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var total = q.LongCount();
            var totalPages = (int)Math.Ceiling(total / (double)pageSize);
            if (totalPages == 0) totalPages = 1; // evita divisão por zero
            var selfPage = Math.Clamp(page, 1, totalPages);

            var pageItems = q
                .Skip((selfPage - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new Usuario.UsuarioResponse(u.UsuarioId, u.Nome, u.Email))
                .ToList();

            // helpers de link (usando rota nomeada acima)
            string? LinkTo(int targetPage) => _links.GetUriByName(
                HttpContext,
                "GetUsuariosPaged",
                new { page = targetPage, pageSize, search, sortBy, sortDir });

            var linkSelf  = LinkTo(selfPage);
            var linkFirst = LinkTo(1);
            var linkLast  = LinkTo(totalPages);
            var linkPrev  = selfPage > 1          ? LinkTo(selfPage - 1) : null;
            var linkNext  = selfPage < totalPages ? LinkTo(selfPage + 1) : null;

            var links = new Dictionary<string, object>();
            if (linkSelf  is not null) links["self"]  = new { href = linkSelf  };
            if (linkFirst is not null) links["first"] = new { href = linkFirst };
            if (linkPrev  is not null) links["prev"]  = new { href = linkPrev  };
            if (linkNext  is not null) links["next"]  = new { href = linkNext  };
            if (linkLast  is not null) links["last"]  = new { href = linkLast  };

            var body = new
            {
                _embedded = new { usuarios = pageItems },
                _links = links,
                page = new
                {
                    size = pageSize,
                    totalElements = total,
                    totalPages,
                    number = selfPage - 1 // zero-based
                }
            };

            return Ok(body);
        }
    }
}
