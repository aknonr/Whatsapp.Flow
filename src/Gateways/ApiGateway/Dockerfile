FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Whatsapp.Flow.Gateways.ApiGateway.csproj", "./"]
RUN dotnet restore "Whatsapp.Flow.Gateways.ApiGateway.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "Whatsapp.Flow.Gateways.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Whatsapp.Flow.Gateways.ApiGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Whatsapp.Flow.Gateways.ApiGateway.dll"] 