# ==================================================
#                      Build 
# ==================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copiar apenas os arquivos de projeto (.csproj) primeiro
# Isso permite que o Docker use cache nas dependências se você não mudou pacotes
COPY ["src/SIPRI.Host/SIPRI.Host.csproj", "src/SIPRI.Host/"]
COPY ["src/SIPRI.Presentation/SIPRI.Presentation.csproj", "src/SIPRI.Presentation/"]
COPY ["src/SIPRI.Application/SIPRI.Application.csproj", "src/SIPRI.Application/"]
COPY ["src/SIPRI.Domain/SIPRI.Domain.csproj", "src/SIPRI.Domain/"]
COPY ["src/SIPRI.Infrastructure/SIPRI.Infrastructure.csproj", "src/SIPRI.Infrastructure/"]

# 2. Baixar as dependências (Restore)
RUN dotnet restore "src/SIPRI.Host/SIPRI.Host.csproj"

# 3. Copiar todo o restante do código fonte
COPY . .

# 4. Compilar e Publicar em modo Release
WORKDIR "/src/src/SIPRI.Host"
RUN dotnet publish "SIPRI.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ==================================================
# Estágio 2: Runtime (Usa a imagem leve do ASP.NET Core para rodar)
# ==================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Expõe as portas
EXPOSE 80
EXPOSE 443

# Copia os artefatos gerados no estágio anterior
COPY --from=build /app/publish .

# Define o comando que inicia a API
ENTRYPOINT ["dotnet", "SIPRI.Host.dll"]