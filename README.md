# MottuGestor

## Projeto: API REST para Gestão de Motos e Pátios - Mottu

Este projeto para o Challenge, da disciplina **DevOps Tools and Cloud Computing** tem como objetivo desenvolver e realizar o deploy de uma **API** utilizando **.NET 8** e **Banco de dados Azure SQL**.

## Integrantes

- Felipe Levy Stephens Fidelix - RM556426
- Jennifer Kaori Suzuki  - RM554661
- Pedro Henrique Jorge de Paula - RM558833

---

## Proposta do Projeto

Criar uma API que permita a gestão completa das motos, pátios e usuários, com funcionalidades para cadastrar, consultar, atualizar e deletar registros.

---

## Estrutura do Projeto

- **API**: Controllers e Validações de entrada
- **Application**: DTOs e Casos de uso
- **Domain**: Entidades, Enums, Value objects e Interfaces
- **Infrastructure**: Acesso a dados e Serviços externos.  

---

## Tecnologias Utilizadas

- .NET 8  
- C#  
- Entity Framework Core (EF Core)  
- SQL Database  
- Swagger / OpenAPI  
- Rider (JetBrains)

---

## Passo a passo do deploy

```bash
# 1. Criar o Resource group
az group create --name rg-dotnet --location brazilsouth

# 2. Criar o Azure SQL Server e o Azure SQL Database
az sql server create -l brazilsouth -g rg-dotnet -n sqlserver-mottugestor -u admsql -p Fiap@2025 --enable-public-network true
az sql db create -g rg-dotnet -s sqlserver-mottugestor -n mottugestordb --service-objective Free --backup-storage-redundancy Local --zone-redundant false

# 3. Adicionar regra de firewall
az sql server firewall-rule create -g rg-dotnet -s sqlserver-mottugestor -n AllowAll --start-ip-address 0.0.0.0 --end-ip-address 255.255.255.255

# 4. Criar o Web App
az webapp create \
  --name mottugestor \
  --resource-group rg-dotnet \
  --plan appservice-dotnet \
  --runtime "DOTNETCORE:8.0" \
  --deployment-source-url "https://github.com/jenniesuzuki/MottuGestor" \
  --deployment-source-branch "main"

# 5. Acessar o banco de dados Azure criado com as credenciais
User: admsql
Password: Fiap@2025

# 6. Criar as tabelas com o script_bd.sql

# 7. Clonar o repositório
git clone https://github.com/jenniesuzuki/MottuGestor.git
cd MottuGestor

# 8. Ajustar a connection string no appsettings.json
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:sqlserver-mottugestor.database.windows.net,1433;Initial Catalog=mottugestordb;Persist Security Info=False;User ID=admsql;Password=Fiap@2025;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }

# 9. Restaurar e dar build no projeto
dotnet restore
dotnet build

# 10. Aplicar migrations
dotnet ef database update --project MottuGestor.Infrastructure --startup-project MottuGestor.API

# 11. Acessar o site após o deploy completo
mottugestor.azurewebsites.net

# 12. Testar as requisições
```

## Endpoints e Testes de exemplo:

### Moto
| Método | Endpoint           | Descrição                       |
|--------|--------------------|--------------------------------|
| GET    | /api/Moto         | Lista todas as motos                        |
| GET    | /api/Moto/{id}    | Consulta moto por ID                        |
| POST   | /api/Moto         | Cadastra nova moto                          |
| PUT    | /api/Moto/{id}    | Atualiza dados de uma moto                  |
| DELETE | /api/Moto/{id}    | Remove uma moto pelo ID                     |
| GET    | /api/Moto/filtro  | Busca motos por modelo (query)              |
| GET    | /api/Moto/paginado| Busca motos com paginação                   |


**Exemplo POST**
```json
{
  "rfidTag": "RFID58760",
  "placa": "APG-4675",
  "modelo": "Fazer 250",
  "marca": "Yamaha",
  "ano": 2023,
  "problema": "Nenhum",
  "localizacao": "Garagem 2"
}
```

**Exemplo PUT**
```json
{
  "rfidTag": "RFID58760",
  "placa": "APG-4675",
  "modelo": "Fazer 250",
  "marca": "Yamaha",
  "ano": 2023,
  "problema": "Nenhum",
  "localizacao": "Garagem 1"
}
```

### Patio
| Método | Endpoint           | Descrição                       |
|--------|--------------------|--------------------------------|
| GET    | /api/Patio         | Lista todos os pátios cadastrados          |
| GET    | /api/Patio/{id}    | Retorna os dados de um pátio pelo ID       |
| POST   | /api/Patio         | Cadastra um novo pátio                     |
| PUT    | /api/Patio/{id}    | Atualiza os dados de um pátio existente    |
| DELETE | /api/Patio/{id}    | Remove um pátio do sistema pelo ID         |
| GET    | /api/Patio/filtro  | Busca pátios pelo nome informado           |
| GET    | /api/Patio/paginado| Busca pátios com paginação                 |

**Exemplo POST**
```json

{
  "nome": "Pátio Butantã",
  "endereco": {
    "rua": "Rua Agostinho Cantu, 209",
    "cidade": "SP",
    "cep": "05501-010"
  },
  "capacidade": 200
}
```
### Usuario
| Método | Endpoint           | Descrição                       |
|--------|--------------------|--------------------------------|
| GET    | /api/Usuario         | Lista todos os usuários cadastrados      |
| GET    | /api/Usuario/{id}    | Consulta usuário por ID                  |
| POST   | /api/Usuario         | Cadastra novo usuário                    |
| PUT    | /api/Usuario/{id}    | Atualiza dados de um usuário             |
| DELETE | /api/Usuario/{id}    | Remove uma usuário pelo ID               |
| GET    | /api/Usuario/filtro  | Busca usuários por modelo (query)        |
| GET    | /api/Usuario/paginado| Busca usuários com paginação             |

**Exemplo POST**
```json
{
  "nome": "Marcos Almeida",
  "email": "marcos@gmail.com",
  "senhaHash": "senha_secreta_marcos"
}
```
