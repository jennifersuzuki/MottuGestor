using MottuGestor.Domain.Enums;
using MottuGestor.Domain.ValueObjects;

namespace MottuGestor.Domain.Entities;


public class Moto
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Placa Placa { get; private set; }
    public string Modelo { get; private set; }
    public string Marca { get; private set; }
    public string RfidTag { get; private set; }
    public int Ano { get; private set; }
    public DateTime DataCadastro { get; private set; } = DateTime.UtcNow;
    public string? Problema { get; private set; }
    public StatusMoto Status { get; private set; } = StatusMoto.Disponivel;


    public Guid? PatioId { get; private set; }


    public Moto(string rfidTag, Placa placa, string modelo, string marca, int ano)
    {
        if (string.IsNullOrWhiteSpace(modelo)) throw new ArgumentException("Modelo obrigatório");
        if (string.IsNullOrWhiteSpace(marca)) throw new ArgumentException("Marca obrigatória");
        if (ano < 1990) throw new ArgumentOutOfRangeException(nameof(ano));


        RfidTag = rfidTag;
        Placa = placa;
        Modelo = modelo.Trim();
        Marca = marca.Trim();
        Ano = ano;
    }


    private Moto() { } // EF


    public void MarcarProblema(string? descricao)
    {
        Problema = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
        if (Problema is not null) Status = StatusMoto.EmManutencao;
        else if (Status == StatusMoto.EmManutencao) Status = StatusMoto.Disponivel;
    }


    internal void SetPatio(Guid patioId) { PatioId = patioId; Status = StatusMoto.EmUso; }
    internal void RemoverDoPatio() { PatioId = null; if (Status == StatusMoto.EmUso) Status = StatusMoto.Disponivel; }
}