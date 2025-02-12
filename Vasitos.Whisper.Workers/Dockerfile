FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Vasitos.Whisper.Workers/Vasitos.Whisper.Workers.csproj", "Vasitos.Whisper.Workers/"]
COPY ["Vasitos.Whisper.Application/Vasitos.Whisper.Application.csproj", "Vasitos.Whisper.Application/"]
COPY ["Vasitos.Whisper.Domain/Vasitos.Whisper.Domain.csproj", "Vasitos.Whisper.Domain/"]
COPY ["Vasitos.Whisper.Infrastructure/Vasitos.Whisper.Infrastructure.csproj", "Vasitos.Whisper.Infrastructure/"]
RUN dotnet restore "Vasitos.Whisper.Workers/Vasitos.Whisper.Workers.csproj"
COPY . .
WORKDIR "/src/Vasitos.Whisper.Workers"
RUN dotnet build "Vasitos.Whisper.Workers.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Vasitos.Whisper.Workers.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Vasitos.Whisper.Workers.dll"]
