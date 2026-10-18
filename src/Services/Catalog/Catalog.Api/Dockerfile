FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Services/Catalog/Catalog.Api/Catalog.Api.csproj                       src/Services/Catalog/Catalog.Api/
COPY src/Services/Catalog/Catalog.Application/Catalog.Application.csproj       src/Services/Catalog/Catalog.Application/
COPY src/Services/Catalog/Catalog.Core/Catalog.Core.csproj                     src/Services/Catalog/Catalog.Core/
COPY src/Services/Catalog/Catalog.Infrastructure/Catalog.Infrastructure.csproj src/Services/Catalog/Catalog.Infrastructure/
RUN dotnet restore src/Services/Catalog/Catalog.Api/Catalog.Api.csproj

COPY src/Services/Catalog/ src/Services/Catalog/
RUN dotnet publish src/Services/Catalog/Catalog.Api/Catalog.Api.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
USER $APP_UID

ENTRYPOINT ["dotnet", "Catalog.Api.dll"]
