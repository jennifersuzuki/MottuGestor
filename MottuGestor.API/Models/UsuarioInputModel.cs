using System.ComponentModel.DataAnnotations;

namespace MottuGestor.API.Models;

public class UsuarioInputModel
{
    [Required]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string SenhaHash { get; set; } = string.Empty;
}