using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MottuGestor.Domain.ValueObjects;

public class EnderecoConverter : ValueConverter<Endereco, string>
{
    public EnderecoConverter() :
        base(
            v => v.ToString(),          // Endereco -> string (para o banco)
            v => Endereco.Parse(v)      // string -> Endereco (ao materializar)
        )
    { }
}