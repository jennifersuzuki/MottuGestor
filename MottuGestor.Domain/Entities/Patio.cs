using MottuGestor.Domain.Enums;

namespace MottuGestor.Domain.Entities;

public class Patio
{
    private readonly List<Moto> _motos = new();


    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Nome { get; private set; }
    public string Endereco { get; private set; }
    public int Capacidade { get; private set; }


    public IReadOnlyCollection<Moto> Motos => _motos.AsReadOnly();


    public Patio(string nome, string endereco, int capacidade)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new ArgumentException("Nome obrigatório");
        if (string.IsNullOrWhiteSpace(endereco)) throw new ArgumentException("Endereço obrigatório");
        if (capacidade <= 0) throw new ArgumentOutOfRangeException(nameof(capacidade));
        Nome = nome.Trim();
        Endereco = endereco.Trim();
        Capacidade = capacidade;
    }


    private Patio() {}


    public void Atualizar(string nome, string endereco, int capacidade)
    {
        if (capacidade < _motos.Count) throw new InvalidOperationException("Capacidade menor que a ocupação atual");
        if (!string.IsNullOrWhiteSpace(nome)) Nome = nome.Trim();
        if (!string.IsNullOrWhiteSpace(endereco)) Endereco = endereco.Trim();
        Capacidade = capacidade;
    }


    public void CheckInMoto(Moto moto)
    {
        if (_motos.Count >= Capacidade) throw new InvalidOperationException("Pátio lotado");
        if (moto is null) throw new ArgumentNullException(nameof(moto));
        if (moto.PatioId == Id) return; // já está aqui
        if (moto.PatioId is not null) throw new InvalidOperationException("Moto já alocada em outro pátio");
        if (moto.Status == StatusMoto.EmManutencao) throw new InvalidOperationException("Moto em manutenção");
        _motos.Add(moto);
        moto.SetPatio(Id);
    }


    public void CheckOutMoto(Moto moto)
    {
        if (!_motos.Remove(moto)) return;
        moto.RemoverDoPatio();
    }
}