# MottuGestor

## Projeto: API RESTful para Gestão de Motos e Pátios - Mottu

Este projeto para o Challenge, da disciplina **Advanced Business Development with .NET** e tem como objetivo desenvolver uma **API RESTful** utilizando **.NET 8** e **Banco de dados Oracle**.

## Integrantes

- Felipe Levy Stephens Fidelix - RM556426
- Jennifer Kaori Suzuki  - RM554661
- Pedro Henrique Jorge de Paula - RM558833

---

## Proposta do Projeto

Criar uma API que permita a gestão completa das motos e pátios da empresa, com funcionalidades para cadastrar, consultar, atualizar e deletar registros.

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
- Oracle Database
- Swagger / OpenAPI  
- Rider (JetBrains)

---

## Passo a passo

```bash
# 1. Clonar o repositório
git clone https://github.com/jenniesuzuki/MottuGestor.git
cd MottuGestor

# 2. Ajustar a connection string no appsettings.json (se desejar)
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:sqlserver-mottugestor.database.windows.net,1433;Initial Catalog=mottugestordb;Persist Security Info=False;User ID=admsql;Password=Fiap@2025;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }

# 3. Restaurar e dar build no projeto
dotnet restore
dotnet build

# 4. Aplicar migrations (se o banco de dados for alterado)
dotnet ef database update --project MottuGestor.Infrastructure --startup-project MottuGestor.API

# 5. Rodar a API
dotnet run --project MottuGestor.API
```

#### Swagger: http://localhost:5183/swagger/index.html


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
  "rfidTag": "RFID52760",
  "placa": "AMF-4675",
  "modelo": "Fazer 250",
  "marca": "Yamaha",
  "ano": 2023,
  "problema": "Nenhum",
  "localizacao": "Garagem 2"
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
  "nome": "Marcos Ferreira",
  "email": "marcosf@gmail.com",
  "senhaHash": "senha_secreta_marcos"
}
```
