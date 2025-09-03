
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

        // Construtor que garante que RFID e demais dados sejam fornecidos ao criar
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

        // Construtor vazio para EF
        public Moto()
        {
            DataCadastro = DateTime.UtcNow;
            Status = StatusMoto.Disponivel;
        }

        // Métodos para alterar campos, se quiser controlar modificações:
        public void AtualizarLocalizacao(string novaLocalizacao)
        {
            Localizacao = novaLocalizacao;
        }

        public void AtualizarProblema(string novoProblema)
        {
            Problema = novoProblema;
        }

        public void AlterarStatus(StatusMoto novoStatus)
        {
            Status = novoStatus;
        }

        public void AtualizarDados(string rfidTag, string placa, string modelo, string marca, int ano, string problema, string localizacao)
        {
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
