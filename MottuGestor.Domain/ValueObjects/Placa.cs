namespace MottuGestor.Domain.ValueObjects;

public class Placa : IEquatable<Placa>
{
    public string Value { get; }


    public Placa(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Placa obrigatória");
        var v = value.Trim().ToUpperInvariant();
        var isValid = System.Text.RegularExpressions.Regex.IsMatch(v, @"^[A-Z]{3}-?\d{4}$")
                      || System.Text.RegularExpressions.Regex.IsMatch(v, @"^[A-Z]{3}\d[A-Z]\d{2}$");
        if (!isValid) throw new ArgumentException("Placa inválida");
        Value = v.Replace("-", "");
    }


    public override string ToString() => Value;
    public bool Equals(Placa? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => Equals(obj as Placa);
    public override int GetHashCode() => Value.GetHashCode();
}