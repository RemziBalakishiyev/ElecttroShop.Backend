# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/ElectroShop.Domain/ElectroShop.Domain.csproj", "src/ElectroShop.Domain/"]
COPY ["src/ElectroShop.Application/ElectroShop.Application.csproj", "src/ElectroShop.Application/"]
COPY ["src/ElectroShop.Persistence/ElectroShop.Persistence.csproj", "src/ElectroShop.Persistence/"]
COPY ["src/ElectroShop.WebApi/ElectroShop.WebApi.csproj", "src/ElectroShop.WebApi/"]

RUN dotnet restore "src/ElectroShop.WebApi/ElectroShop.WebApi.csproj"

COPY . .
WORKDIR "/src/src/ElectroShop.WebApi"
RUN dotnet build "ElectroShop.WebApi.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ElectroShop.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:10000

EXPOSE 10000

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ElectroShop.WebApi.dll"]
