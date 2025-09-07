using MottuGestor.Domain.ValueObjects;

namespace MottuGestor.Domain.Entities
{
    public class Patio
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public Endereco Endereco { get; set; }
        public int Capacidade { get; set; }

        public Patio(string nome, Endereco endereco, int capacidade)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Endereco = endereco;
            Capacidade = capacidade;
        }
        
        public Patio() { }

        public void AtualizarDados(string nome, Endereco endereco, int capacidade)
        {
            if (endereco is null) throw new Exception("Endereço obrigatório.");
            if (capacidade < 0) throw new Exception("Capacidade não pode ser negativa.");
            Nome = nome;
            Endereco = endereco;
            Capacidade = capacidade;
        }
    }
}