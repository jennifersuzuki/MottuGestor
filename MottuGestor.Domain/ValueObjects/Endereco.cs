namespace MottuGestor.Domain.ValueObjects;

public class Endereco : ValueObject<Endereco>
{
    public string Rua { get; }
    public string Cidade { get; }
    public string Cep { get; }
    
    public Endereco(string rua, string cidade, string cep)
    {
        Rua = rua;
        Cidade = cidade;
        Cep = cep;
    }
    
    public override string ToString() => $"{Rua}|{Cidade}|{Cep}";
    public static Endereco Parse(string persisted)
    {
        if (string.IsNullOrWhiteSpace(persisted)) return new Endereco("", "", "");
        var parts = persisted.Split('|');
        return new Endereco(
            parts.ElementAtOrDefault(0)?.Trim() ?? "",
            parts.ElementAtOrDefault(1)?.Trim() ?? "",
            parts.ElementAtOrDefault(2)?.Trim() ?? ""
        );
    }

    protected override bool EqualsCore(Endereco other)
    {
        return Rua == other.Rua
               && Cidade == other.Cidade
               && Cep == other.Cep;
    }

    protected override int GetHashCodeCore()
    {
        unchecked
        {
            int hashCode = Rua.GetHashCode();
            hashCode = (hashCode * 397) ^ Cidade.GetHashCode();
            hashCode = (hashCode * 397) ^ Cep.GetHashCode();
            return hashCode;
        }
    }
}