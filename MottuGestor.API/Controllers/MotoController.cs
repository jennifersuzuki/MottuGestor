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

        public MotoController(IRepository<Moto> motoRepository)
        {
            _motoRepository = motoRepository;
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
        
        [HttpGet("paginado")]
        [ProducesResponseType(typeof(PageResult<Moto.MotoResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PageResult<Moto.MotoResponse>>> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = "DataCadastro",
            [FromQuery] string? sortDir = "Desc"
        )
        {

            var all = await _motoRepository.GetAllAsync();
            var q = all.AsQueryable();
            
            var asc = string.Equals(sortDir, "Asc", StringComparison.OrdinalIgnoreCase);
            q = (sortBy?.ToLowerInvariant()) switch
            {
                "placa"        => asc ? q.OrderBy(m => m.Placa)        : q.OrderByDescending(m => m.Placa),
                "modelo"       => asc ? q.OrderBy(m => m.Modelo)       : q.OrderByDescending(m => m.Modelo),
                "marca"        => asc ? q.OrderBy(m => m.Marca)        : q.OrderByDescending(m => m.Marca),
                "ano"          => asc ? q.OrderBy(m => m.Ano)          : q.OrderByDescending(m => m.Ano),
                "status"       => asc ? q.OrderBy(m => m.Status)       : q.OrderByDescending(m => m.Status),
                "datacadastro" => asc ? q.OrderBy(m => m.DataCadastro) : q.OrderByDescending(m => m.DataCadastro),
                _              => asc ? q.OrderBy(m => m.DataCadastro) : q.OrderByDescending(m => m.DataCadastro),
            };
            
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var total = q.LongCount();

            var pageEntities = q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var items = pageEntities
                .Select(m => new Moto.MotoResponse(
                    m.MotoId,
                    m.Placa,
                    m.Modelo,
                    m.RfidTag,
                    m.Status.ToString(),
                    m.Localizacao ?? string.Empty
                ))
                .ToList();

            var result = new PageResult<Moto.MotoResponse>
            {
                Items = items,
                Total = (int)total,
                HasMore = page * pageSize < total,
                Page = page,
                PageSize = pageSize
            };

            return Ok(result);
        }

    }
}
