
using MottuGestor.Domain.Enums;

namespace MottuGestor.Domain.Entities
{
    public class Moto
    {
        public Guid MotoId { get; private set; }
        public string Placa { get; private set; } = string.Empty;
        public string Modelo { get; private set; } = string.Empty;
        public string Marca { get; private set; } = string.Empty;
        public string RfidTag { get; private set; } = string.Empty;
        public int Ano { get; private set; }
        public DateTime DataCadastro { get; private set; }
        public string Problema { get; private set; } = string.Empty;
        public string Localizacao { get; private set; } = string.Empty;
        public StatusMoto Status { get; private set; }
        
        public Moto(string rfidTag, string placa, string modelo, string marca, int ano, string problema = null, string localizacao = null)
        {
            MotoId = Guid.NewGuid();
            RfidTag = ValidateRfid(rfidTag);
            Placa = placa;
            Modelo = modelo;
            Marca = marca;
            Ano = ano;
            Problema = problema;
            Localizacao = localizacao;
            DataCadastro = DateTime.UtcNow;
            Status = StatusMoto.Disponivel;
        }

        // Validação simples para RFID
        private string ValidateRfid(string rfid)
        {
            if (string.IsNullOrWhiteSpace(rfid))
                throw new ArgumentException("RfidTag não pode ser vazia.");

            return rfid;
        }
        public Moto()
        {
            DataCadastro = DateTime.UtcNow;
            Status = StatusMoto.Disponivel;
        }

        public void AtualizarDados(string rfidTag, string placa, string modelo, string marca, int ano, string problema, string localizacao)
        {
            if (ano < 1800) throw new Exception("Ano mínimo é 1800.");
            
            RfidTag = rfidTag;
            Placa = placa;
            Modelo = modelo;
            Marca = marca;
            Ano = ano;
            Problema = problema;
            Localizacao = localizacao;
        }


    }
}
