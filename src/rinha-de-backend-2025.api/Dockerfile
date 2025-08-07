FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/rinha-de-backend-2025.api/rinha-de-backend-2025.api.csproj", "src/rinha-de-backend-2025.api/"]
RUN dotnet restore "src/rinha-de-backend-2025.api/rinha-de-backend-2025.api.csproj"
COPY . .
WORKDIR "/src/src/rinha-de-backend-2025.api"
RUN dotnet build "./rinha-de-backend-2025.api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./rinha-de-backend-2025.api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "rinha-de-backend-2025.api.dll"]
