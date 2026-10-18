FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/BuildingBlocks/BuildingBlocks.csproj                               src/BuildingBlocks/
COPY src/Services/Orders/Orders.Api/Orders.Api.csproj                       src/Services/Orders/Orders.Api/
COPY src/Services/Orders/Orders.Application/Orders.Application.csproj       src/Services/Orders/Orders.Application/
COPY src/Services/Orders/Orders.Core/Orders.Core.csproj                     src/Services/Orders/Orders.Core/
COPY src/Services/Orders/Orders.Infrastructure/Orders.Infrastructure.csproj src/Services/Orders/Orders.Infrastructure/
RUN dotnet restore src/Services/Orders/Orders.Api/Orders.Api.csproj

COPY src/BuildingBlocks/ src/BuildingBlocks/
COPY src/Services/Orders/ src/Services/Orders/
RUN dotnet publish src/Services/Orders/Orders.Api/Orders.Api.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "Orders.Api.dll"]
