# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 7000

# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем файлы проектов для восстановления зависимостей (кеширование слоёв)
COPY ["backend/src/MusicianFinder.API/MusicianFinder.API.csproj", "backend/src/MusicianFinder.API/"]
COPY ["backend/src/MusicianFinder.Application/MusicianFinder.Application.csproj", "backend/src/MusicianFinder.Application/"]
COPY ["backend/src/MusicianFinder.Domain/MusicianFinder.Domain.csproj", "backend/src/MusicianFinder.Domain/"]
COPY ["backend/src/MusicianFinder.Infrastructure/MusicianFinder.Infrastructure.csproj", "backend/src/MusicianFinder.Infrastructure/"]
COPY ["backend/src/MusicianFinder.SharedKernel/MusicianFinder.SharedKernel.csproj", "backend/src/MusicianFinder.SharedKernel/"]

# Восстанавливаем зависимости
RUN dotnet restore "backend/src/MusicianFinder.API/MusicianFinder.API.csproj"

# Копируем остальные исходные файлы
COPY . .

# Собираем проект
WORKDIR "/src/backend/src/MusicianFinder.API"
RUN dotnet build "MusicianFinder.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MusicianFinder.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MusicianFinder.API.dll"]