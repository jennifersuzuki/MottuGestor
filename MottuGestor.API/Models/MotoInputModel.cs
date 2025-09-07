using System.ComponentModel.DataAnnotations;

namespace MottuGestor.API.Models
{
    public class MotoInputModel
    {
        [Required]
        public required string RfidTag { get; set; }

        [Required, RegularExpression(@"^(?:[A-Z]{3}-?\d{4}|[A-Z]{3}\d[A-Z]\d{2})$", 
             ErrorMessage = "Placa inválida (use AAA-1234 ou AAA1A23).")]
        public required string Placa { get; set; }

        [Required]
        public required string Modelo { get; set; }

        [Required]
        public required string Marca { get; set; }

        [Required, Range(1800, 2026, ErrorMessage = "Ano deve ser >= 1800.")]
        public int Ano { get; set; }

        public string? Problema { get; set; }

        public string? Localizacao { get; set; }
    }
}
