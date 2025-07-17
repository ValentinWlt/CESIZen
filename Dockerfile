# Dockerfile pour CESIZen
# Application ASP.NET Core 8.0 MVC

# Image de base pour l'exécution (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Image pour la construction (build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copier les fichiers de projet et restaurer les dépendances
COPY ["CESIZen/CESIZen.csproj", "CESIZen/"]
RUN dotnet restore "CESIZen/CESIZen.csproj"

# Copier tout le code source
COPY . .
WORKDIR "/src/CESIZen"

# Construire l'application
RUN dotnet build "CESIZen.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Phase de publication
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CESIZen.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Image finale pour l'exécution
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CESIZen.dll"]