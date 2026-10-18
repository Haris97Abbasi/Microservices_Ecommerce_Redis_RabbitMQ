FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/BuildingBlocks/BuildingBlocks.csproj                                               src/BuildingBlocks/
COPY src/Services/Notifications/Notifications.Api/Notifications.Api.csproj                       src/Services/Notifications/Notifications.Api/
COPY src/Services/Notifications/Notifications.Application/Notifications.Application.csproj       src/Services/Notifications/Notifications.Application/
COPY src/Services/Notifications/Notifications.Core/Notifications.Core.csproj                     src/Services/Notifications/Notifications.Core/
COPY src/Services/Notifications/Notifications.Infrastructure/Notifications.Infrastructure.csproj src/Services/Notifications/Notifications.Infrastructure/
RUN dotnet restore src/Services/Notifications/Notifications.Api/Notifications.Api.csproj

COPY src/BuildingBlocks/ src/BuildingBlocks/
COPY src/Services/Notifications/ src/Services/Notifications/
RUN dotnet publish src/Services/Notifications/Notifications.Api/Notifications.Api.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "Notifications.Api.dll"]
