using System.Net;
using Microsoft.AspNetCore.Mvc;
using MottuGestor.API.Models;
using MottuGestor.Domain.Entities;
using MottuGestor.Domain.Enums;
using MottuGestor.Domain.Pagination;
using MottuGestor.Infrastructure.Repositories;

namespace MottuGestor.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MotoController : ControllerBase
    {
        private readonly IRepository<Moto> _motoRepository;
        private readonly IMotoRepository _motosRepository;
        private readonly LinkGenerator _links;

        public MotoController(IRepository<Moto> motoRepository, IMotoRepository motosRepository, LinkGenerator links)
        {
            _motoRepository = motoRepository;
            _motosRepository = motosRepository ?? throw new ArgumentNullException(nameof(motosRepository));
            _links = links ?? throw new ArgumentNullException(nameof(links));
        }

        // ============================
        // [GET] /moto
        // Retorna todas as motos cadastradas
        // ============================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var motos = await _motoRepository.GetAllAsync();
            return Ok(motos);
        }

        // ============================
        // [GET] /moto/{id}
        // Busca uma moto específica por ID
        // ============================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var moto = await _motoRepository.GetByIdAsync(id);
            if (moto == null)
                return NotFound("Moto não encontrada.");

            return Ok(moto);
        }

        // ============================
        // [GET] /moto/{filtro}
        // Busca motos por modelo (query)
        // ============================
        [HttpGet("filtro")]
        public async Task<IActionResult> Filtrar(
    [FromQuery] StatusMoto? status,
    [FromQuery] string? marca,
    [FromQuery] int? ano)
        {
            var motos = await _motoRepository.GetAllAsync();

            if (status.HasValue)
                motos = motos.Where(m => m.Status == status).ToList();

            if (!string.IsNullOrEmpty(marca))
                motos = motos.Where(m => m.Marca.ToLower().Contains(marca.ToLower())).ToList();

            if (ano.HasValue)
                motos = motos.Where(m => m.Ano == ano).ToList();

            return Ok(motos);
        }

        // ============================
        // [POST] /moto
        // Cria uma nova moto
        // ============================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MotoInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var moto = new Moto(
                    rfidTag: input.RfidTag,
                    placa: input.Placa,
                    modelo: input.Modelo,
                    marca: input.Marca,
                    ano: input.Ano,
                    problema: input.Problema,
                    localizacao: input.Localizacao
                );

                await _motoRepository.AddAsync(moto);
                await _motoRepository.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = moto.MotoId }, moto);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao cadastrar moto: {ex.Message}");
            }
        }

        // ============================
        // [PUT] /moto/{id}
        // Atualiza os dados de uma moto existente
        // ============================
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Update(Guid id, [FromBody] MotoInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var motoExistente = await _motoRepository.GetByIdAsync(id);
            if (motoExistente == null)
                return NotFound("Moto não encontrada.");

            try
            {
                motoExistente.AtualizarDados(
                    rfidTag: input.RfidTag,
                    placa: input.Placa,
                    modelo: input.Modelo,
                    marca: input.Marca,
                    ano: input.Ano,
                    problema: input.Problema,
                    localizacao: input.Localizacao
                );

                await _motoRepository.UpdateAsync(motoExistente);
                await _motoRepository.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar moto: {ex.Message}");
            }
        }


        // ============================
        // [DELETE] /moto/{id}
        // Remove uma moto pelo ID
        // ============================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var moto = await _motoRepository.GetByIdAsync(id);
            if (moto == null)
                return NotFound("Moto não encontrada.");

            try
            {
                await _motoRepository.DeleteAsync(id);
                await _motoRepository.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao excluir moto: {ex.Message}");
            }
        }
        
        [HttpGet("paginado", Name = "GetMotosPaged")]
        [Produces("application/hal+json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = "DataCadastro",
            [FromQuery] string? sortDir = "Desc",
            CancellationToken ct = default)
        {
            // 1) Busca já paginada no repo
            var pr = await _motosRepository.GetPaginationAsyncMoto(page, pageSize, ct);
            pr ??= new PageResult<Moto> { Items = Array.Empty<Moto>(), Page = page, PageSize = pageSize, Total = 0 };

            var items = pr.Items.Select(m => new Moto.MotoResponse(
                m.MotoId,
                m.Placa,
                m.Modelo,
                m.RfidTag,
                m.Status.ToString(),
                m.Localizacao ?? string.Empty
            )).ToList();

            // 2) Cálculo de páginas
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var total = pr.Total;
            var totalPages = (int)Math.Ceiling(total / (double)pageSize);
            if (totalPages == 0) totalPages = 1;

            var selfPage = Math.Clamp(page, 1, totalPages);

            // 3) Helper de link (usa a rota nomeada acima)
            string? LinkTo(int targetPage)
            {
                return _links.GetUriByName(
                    HttpContext,
                    "GetMotosPaged",
                    values: new
                    {
                        page = targetPage,
                        pageSize,
                        search,
                        sortBy,
                        sortDir
                    });
            }

            // 4) Monta links só se conseguir gerar a URL
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
                _embedded = new { motos = items },
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
