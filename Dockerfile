FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["ConstructionManager.Api/ConstructionManager.Api.csproj", "ConstructionManager.Api/"]
RUN dotnet restore "ConstructionManager.Api/ConstructionManager.Api.csproj"

COPY . .
WORKDIR "/src/ConstructionManager.Api"
RUN dotnet publish "ConstructionManager.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ConstructionManager.Api.dll"]