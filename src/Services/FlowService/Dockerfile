FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["API/Whatsapp.Flow.Services.Flow.API.csproj", "API/"]
COPY ["Application/Whatsapp.Flow.Services.Flow.Application.csproj", "Application/"]
COPY ["Domain/Whatsapp.Flow.Services.Flow.Domain.csproj", "Domain/"]
COPY ["Infrastructure/Whatsapp.Flow.Services.Flow.Infrastructure.csproj", "Infrastructure/"]
COPY ["../BuildingBlocks/Common/Whatsapp.Flow.BuildingBlocks.Common.csproj", "../BuildingBlocks/Common/"]
COPY ["../BuildingBlocks/EventBus/Whatsapp.Flow.BuildingBlocks.EventBus.csproj", "../BuildingBlocks/EventBus/"]
COPY ["../BuildingBlocks/EventBus.RabbitMQ/Whatsapp.Flow.BuildingBlocks.EventBus.RabbitMQ.csproj", "../BuildingBlocks/EventBus.RabbitMQ/"]

RUN dotnet restore "API/Whatsapp.Flow.Services.Flow.API.csproj"
COPY . .
WORKDIR "/src/API"
RUN dotnet build "Whatsapp.Flow.Services.Flow.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Whatsapp.Flow.Services.Flow.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Whatsapp.Flow.Services.Flow.API.dll"] 