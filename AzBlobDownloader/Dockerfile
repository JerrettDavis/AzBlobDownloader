FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AzBlobDownloader/AzBlobDownloader.csproj", "AzBlobDownloader/"]
RUN dotnet restore "AzBlobDownloader/AzBlobDownloader.csproj"
COPY . .
WORKDIR "/src/AzBlobDownloader"
RUN dotnet build "AzBlobDownloader.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AzBlobDownloader.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AzBlobDownloader.dll"]
