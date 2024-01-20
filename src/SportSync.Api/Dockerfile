#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/SportSync.Api/SportSync.Api.csproj", "src/SportSync.Api/"]
COPY ["src/SportSync.Infrastructure/SportSync.Infrastructure.csproj", "src/SportSync.Infrastructure/"]
COPY ["src/SportSync.Application/SportSync.Application.csproj", "src/SportSync.Application/"]
COPY ["src/SportSync.Domain/SportSync.Domain.csproj", "src/SportSync.Domain/"]
COPY ["src/SportSync.Persistence/SportSync.Persistence.csproj", "src/SportSync.Persistence/"]
RUN dotnet restore "src/SportSync.Api/SportSync.Api.csproj"
COPY . .
WORKDIR "/src/src/SportSync.Api"
RUN dotnet build "SportSync.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SportSync.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SportSync.Api.dll"]