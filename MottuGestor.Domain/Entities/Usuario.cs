using System;

namespace MottuGestor.Domain.Entities
{
    public class Usuario
    {
        public Guid UsuarioId { get; private set; }
        public string Nome { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string SenhaHash { get; private set; } = string.Empty;
        public DateTime DataCadastro { get; private set; }

        // Construtor principal
        public Usuario(string nome, string email, string senhaHash)
        {
            UsuarioId = Guid.NewGuid();
            Nome = ValidarNome(nome);
            Email = ValidarEmail(email);
            SenhaHash = ValidarSenha(senhaHash);
            DataCadastro = DateTime.UtcNow;
        }

        // Construtor vazio para EF
        public Usuario()
        {
            DataCadastro = DateTime.UtcNow;
        }

        // Atualização controlada
        public void AtualizarNome(string novoNome)
        {
            Nome = ValidarNome(novoNome);
        }

        public void AtualizarEmail(string novoEmail)
        {
            Email = ValidarEmail(novoEmail);
        }

        public void AtualizarSenha(string novaSenhaHash)
        {
            SenhaHash = ValidarSenha(novaSenhaHash);
        }

        // Validações simples
        private string ValidarNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome não pode ser vazio.");
            return nome.Trim();
        }

        private string ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                throw new ArgumentException("Email inválido.");
            return email.Trim();
        }

        private string ValidarSenha(string senhaHash)
        {
            if (string.IsNullOrWhiteSpace(senhaHash))
                throw new ArgumentException("Senha não pode ser vazia.");
            return senhaHash;
        }

    }
}