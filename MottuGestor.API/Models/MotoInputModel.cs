using System.ComponentModel.DataAnnotations;

namespace MottuGestor.API.Models
{
    public class MotoInputModel
    {
        [Required]
        public required string RfidTag { get; set; }

        [Required]
        public required string Placa { get; set; }

        [Required]
        public required string Modelo { get; set; }

        [Required]
        public required string Marca { get; set; }

        [Required]
        public int Ano { get; set; }

        public string? Problema { get; set; }

        public string? Localizacao { get; set; }
    }
}
