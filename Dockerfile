FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PaymentIntegration.Api/PaymentIntegration.Api.csproj", "PaymentIntegration.Api/"]
COPY ["PaymentIntegration.Application/PaymentIntegration.Application.csproj", "PaymentIntegration.Application/"]
COPY ["PaymentIntegration.Domain/PaymentIntegration.Domain.csproj", "PaymentIntegration.Domain/"]
COPY ["PaymentIntegration.Infrastructure/PaymentIntegration.Infrastructure.csproj", "PaymentIntegration.Infrastructure/"]
RUN dotnet restore "PaymentIntegration.Api/PaymentIntegration.Api.csproj"
COPY . .
WORKDIR "/src/PaymentIntegration.Api"
RUN dotnet build "PaymentIntegration.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PaymentIntegration.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentIntegration.Api.dll"] 