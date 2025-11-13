# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/ElectroShop.Domain/ElectroShop.Domain.csproj", "src/ElectroShop.Domain/"]
COPY ["src/ElectroShop.Application/ElectroShop.Application.csproj", "src/ElectroShop.Application/"]
COPY ["src/ElectroShop.Persistence/ElectroShop.Persistence.csproj", "src/ElectroShop.Persistence/"]
COPY ["src/ElectroShop.WebApi/ElectroShop.WebApi.csproj", "src/ElectroShop.WebApi/"]

RUN dotnet restore "src/ElectroShop.WebApi/ElectroShop.WebApi.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/ElectroShop.WebApi"
RUN dotnet build "ElectroShop.WebApi.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ElectroShop.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install PostgreSQL client for migrations (optional)
RUN apt-get update && \
    apt-get install -y postgresql-client && \
    rm -rf /var/lib/apt/lists/*

EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ElectroShop.WebApi.dll"]
