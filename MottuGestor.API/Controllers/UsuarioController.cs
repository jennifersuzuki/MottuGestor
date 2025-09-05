using System.Net;
using Microsoft.AspNetCore.Mvc;
using MottuGestor.API.Models;
using MottuGestor.Domain.Entities;
using MottuGestor.Infrastructure.Repositories;

namespace MottuGestor.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IRepository<Usuario> _usuarioRepository;

        public UsuarioController(IRepository<Usuario> usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
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

                return NoContent();
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
    }
}
