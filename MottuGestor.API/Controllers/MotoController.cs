using System.Net;
using Microsoft.AspNetCore.Mvc;
using MottuGestor.API.Models;
using MottuGestor.Domain.Entities;
using MottuGestor.Domain.Enums;
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
    }
}
