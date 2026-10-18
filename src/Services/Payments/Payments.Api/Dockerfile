FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/BuildingBlocks/BuildingBlocks.csproj                                     src/BuildingBlocks/
COPY src/Services/Payments/Payments.Api/Payments.Api.csproj                       src/Services/Payments/Payments.Api/
COPY src/Services/Payments/Payments.Application/Payments.Application.csproj       src/Services/Payments/Payments.Application/
COPY src/Services/Payments/Payments.Core/Payments.Core.csproj                     src/Services/Payments/Payments.Core/
COPY src/Services/Payments/Payments.Infrastructure/Payments.Infrastructure.csproj src/Services/Payments/Payments.Infrastructure/
RUN dotnet restore src/Services/Payments/Payments.Api/Payments.Api.csproj

COPY src/BuildingBlocks/ src/BuildingBlocks/
COPY src/Services/Payments/ src/Services/Payments/
RUN dotnet publish src/Services/Payments/Payments.Api/Payments.Api.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "Payments.Api.dll"]
