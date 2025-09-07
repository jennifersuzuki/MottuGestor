using System.ComponentModel.DataAnnotations;
using MottuGestor.Domain.ValueObjects;

namespace MottuGestor.API.Models
{
    public class PatioInputModel
    {
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Required]
        public Endereco Endereco { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Capacidade deve ser maior que zero.")]
        public int Capacidade { get; set; }
    }
}
