FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/API/EventRegistrationSystem.API.csproj", "src/API/"]
COPY ["src/Core/Application/EventRegistrationSystem.Application.csproj", "src/Core/Application/"]
COPY ["src/Core/Domain/EventRegistrationSystem.Domain.csproj", "src/Core/Domain/"]
COPY ["src/Infrastructure/EventRegistrationSystem.Infrastructure/EventRegistrationSystem.Infrastructure.csproj", "src/Infrastructure/EventRegistrationSystem.Infrastructure/"]
COPY ["src/Infrastructure/EventRegistrationSystem.Persistence/EventRegistrationSystem.Persistence.csproj", "src/Infrastructure/EventRegistrationSystem.Persistence/"]

RUN dotnet restore "src/API/EventRegistrationSystem.API.csproj"
COPY . .
WORKDIR "/src/src/API"
RUN dotnet build "EventRegistrationSystem.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EventRegistrationSystem.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EventRegistrationSystem.API.dll"]
