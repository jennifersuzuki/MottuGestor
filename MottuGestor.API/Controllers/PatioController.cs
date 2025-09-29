using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuGestor.API.Models;
using MottuGestor.Domain.Entities;
using MottuGestor.Domain.Pagination;
using MottuGestor.Infrastructure.Repositories;

namespace MottuGestor.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PatioController : ControllerBase
    {
        private readonly IRepository<Patio> _patioRepository;
        private readonly LinkGenerator _links;

        public PatioController(IRepository<Patio> patioRepository, LinkGenerator links)
        {
            _patioRepository = patioRepository;
            _links = links;
        }

        // [GET] /patio
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var patios = await _patioRepository.GetAllAsync();
            return Ok(patios);
        }

        // [GET] /patio/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var patio = await _patioRepository.GetByIdAsync(id);
            if (patio == null)
                return NotFound("Patio não encontrado.");

            return Ok(patio);
        }

        // [GET] /patio/filtro
        // Filtra pátios por nome, endereço ou capacidade mínima
        [HttpGet("filtro")]
        public async Task<IActionResult> Filtrar(
            [FromQuery] string? nome,
            [FromQuery] string? endereco,
            [FromQuery] int? capacidadeMinima)
        {
            var patios = await _patioRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(nome))
                patios = patios.Where(p => p.Nome.ToLower().Contains(nome.ToLower())).ToList();

            if (!string.IsNullOrWhiteSpace(endereco))
                patios = patios.Where(p => p.Endereco.ToString().ToLower().Contains(endereco.ToLower())).ToList();

            if (capacidadeMinima.HasValue)
                patios = patios.Where(p => p.Capacidade >= capacidadeMinima).ToList();

            return Ok(patios);
        }


        // [POST] /patio
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PatioInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var patio = new Patio(
                    nome: input.Nome,
                    endereco: input.Endereco,
                    capacidade: input.Capacidade
                );

                await _patioRepository.AddAsync(patio);
                await _patioRepository.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = patio.Id }, patio);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao cadastrar pátio: {ex.Message}");
            }
        }

        // [PUT] /patio/{id}
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Update(Guid id, [FromBody] PatioInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var patioExistente = await _patioRepository.GetByIdAsync(id);
            if (patioExistente == null)
                return NotFound("Pátio não encontrado.");

            try
            {
                patioExistente.AtualizarDados(
                    nome: input.Nome,
                    endereco: input.Endereco,
                    capacidade: input.Capacidade
                );

                await _patioRepository.UpdateAsync(patioExistente);
                await _patioRepository.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar pátio: {ex.Message}");
            }
        }

        // [DELETE] /patio/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var patio = await _patioRepository.GetByIdAsync(id);
            if (patio == null)
                return NotFound("Pátio não encontrado.");

            try
            {
                await _patioRepository.DeleteAsync(id);
                await _patioRepository.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao excluir pátio: {ex.Message}");
            }
        }
        
        [HttpGet("paginado", Name = "GetPatiosPaged")]
        [Produces("application/hal+json")]
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
            var all = await _patioRepository.GetAllAsync();
            var q = all.AsQueryable();

            // FILTRO (sem ?. dentro da expressão)
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = $"%{search.Trim()}%";
                q = q.Where(p =>
                    EF.Functions.Like((p.Nome ?? string.Empty), s) ||
                    (p.Endereco != null && (
                        EF.Functions.Like((p.Endereco.Rua    ?? string.Empty), s) ||
                        EF.Functions.Like((p.Endereco.Cidade ?? string.Empty), s) ||
                        EF.Functions.Like((p.Endereco.Cep    ?? string.Empty), s)
                    ))
                );
            }

            // ORDENAÇÃO
            var asc = string.Equals(sortDir, "Asc", StringComparison.OrdinalIgnoreCase);
            q = (sortBy?.ToLowerInvariant()) switch
            {
                "capacidade" => asc ? q.OrderBy(p => p.Capacidade) : q.OrderByDescending(p => p.Capacidade),
                "id"         => asc ? q.OrderBy(p => p.Id)         : q.OrderByDescending(p => p.Id),
                _            => asc ? q.OrderBy(p => p.Nome)       : q.OrderByDescending(p => p.Nome),
            };

            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var total = q.LongCount();
            var totalPages = (int)Math.Ceiling(total / (double)pageSize);
            if (totalPages == 0) totalPages = 1;
            var selfPage = Math.Clamp(page, 1, totalPages);

            // PROJEÇÃO (sem ?. — use operador ternário)
            var pageSlice = q
                .Skip((selfPage - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    p.Id,
                    p.Nome,
                    Rua    = (p.Endereco != null ? p.Endereco.Rua    : string.Empty),
                    Cidade = (p.Endereco != null ? p.Endereco.Cidade : string.Empty),
                    Cep    = (p.Endereco != null ? p.Endereco.Cep    : string.Empty),
                    p.Capacidade
                })
                .ToList();

            var items = pageSlice
                .Select(t => new Patio.PatioResponse(
                    t.Id,
                    t.Nome,
                    $"{t.Rua}|{t.Cidade}|{t.Cep}",
                    t.Capacidade))
                .ToList();

            // LINKS HATEOAS
            string? LinkTo(int targetPage) => _links.GetUriByName(
                HttpContext,
                "GetPatiosPaged",
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
                _embedded = new { patios = items },
                _links = links,
                page = new
                {
                    size = pageSize,
                    totalElements = total,
                    totalPages,
                    number = selfPage - 1
                }
            };

            return Ok(body);
        }

    }
}
