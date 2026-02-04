FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["App.Presentation/App.Presentation.csproj", "App.Presentation/"]
COPY ["App.Application/App.Application.csproj", "App.Application/"]
COPY ["App.Domain/App.Domain.csproj", "App.Domain/"]
COPY ["App.Shared/App.Shared.csproj", "App.Shared/"]
COPY ["App.Infrastructure/App.Infrastructure.csproj", "App.Infrastructure/"]
RUN dotnet restore "App.Presentation/App.Presentation.csproj"
COPY . .
WORKDIR "/src/App.Presentation"
RUN dotnet build "./App.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./App.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "App.Presentation.dll"]
