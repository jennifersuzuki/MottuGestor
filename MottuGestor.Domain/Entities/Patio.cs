namespace MottuGestor.Domain.Entities
{
    public class Patio
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public int Capacidade { get; set; }

        public Patio(string nome, string endereco, int capacidade)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Endereco = endereco;
            Capacidade = capacidade;
        }

        // Construtor vazio para EF
        public Patio() { }

        public void AtualizarDados(string nome, string endereco, int capacidade)
        {
            Nome = nome;
            Endereco = endereco;
            Capacidade = capacidade;
        }
    }
}