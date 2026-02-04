# 1. AŞAMA: BUILD (SDK imajı kullanılır, derleme yapılır)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Proje dosyalarını kopyala (Cache optimizasyonu için önce csproj'lar)
COPY ["App.Presentation/App.Presentation.csproj", "App.Presentation/"]
COPY ["App.Infrastructure/App.Infrastructure.csproj", "App.Infrastructure/"]
COPY ["App.Application/App.Application.csproj", "App.Application/"]
COPY ["App.Domain/App.Domain.csproj", "App.Domain/"]
COPY ["App.Shared/App.Shared.csproj", "App.Shared/"]

# Bağımlılıkları yükle
RUN dotnet restore "App.Presentation/App.Presentation.csproj"

# Tüm kodları kopyala
COPY . .

# Derle ve Yayınla (Publish)
WORKDIR "/src/App.Presentation"
RUN dotnet publish "App.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 2. AŞAMA: RUNTIME (Sadece çalışma zamanı dosyaları alınır - Hafiftir)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080

# Build aşamasından çıkan dosyaları al
COPY --from=build /app/publish .

# Uygulamayı başlat
ENTRYPOINT ["dotnet", "App.Presentation.dll"]