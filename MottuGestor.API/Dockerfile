# --- Estágio 1: Build (Compilação) ---
# Usa a imagem SDK (Software Development Kit) que tem todas as ferramentas de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 1. Copia o arquivo .sln e TODOS os arquivos .csproj
# Isso permite ao 'dotnet restore' funcionar corretamente
# (Ajuste os nomes dos arquivos .csproj se forem diferentes)
COPY ["MottuGestor.sln", "."]
COPY ["MottuGestor.API/MottuGestor.API.csproj", "MottuGestor.API/"]
COPY ["MottuGestor.Application/MottuGestor.Application.csproj", "MottuGestor.Application/"]
COPY ["MottuGestor.Domain/MottuGestor.Domain.csproj", "MottuGestor.Domain/"]
COPY ["MottuGestor.Infrastructure/MottuGestor.Infrastructure.csproj", "MottuGestor.Infrastructure/"]

RUN dotnet restore "MottuGestor.sln"

COPY . .

WORKDIR "/src/MottuGestor.API"
RUN dotnet publish "MottuGestor.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MottuGestor.API.dll"]
