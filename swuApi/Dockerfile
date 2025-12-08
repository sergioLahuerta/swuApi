FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["swuApi.csproj", "."]
RUN dotnet restore
COPY . .
# Publica la aplicación, minimizando el tamaño del binario
RUN dotnet publish "swuApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

# Comando de ejecución
ENTRYPOINT ["dotnet", "swuApi.dll"]