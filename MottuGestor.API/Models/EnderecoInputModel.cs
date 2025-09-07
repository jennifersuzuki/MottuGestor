using System.ComponentModel.DataAnnotations;

namespace MottuGestor.API.Models;

public class EnderecoInputModel
{
    [Required, StringLength(120)]
    public string Rua { get; set; } = null!;
    
    [Required, StringLength(80)]
    public string Cidade { get; set; } = null!;
    
    [Required, RegularExpression(@"^\d{8}$")]
    public string Cep { get; set; } = null!;
}