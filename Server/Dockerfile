FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["{{Jouw WebAppFileNaam}}.csproj", "WebApp/"]
COPY ["{{Eventueel locaties van andere dll projecten}}.csproj", "{{Locatie die je gebruikt in je WebApp}}/"]
RUN dotnet restore "WebApp/{{Jouw WebAppFileNaam}}.csproj"
COPY . .
WORKDIR "/src/WebApp"
RUN dotnet publish "{{Jouw WebAppFileNaam}}.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "{{Jouw WebAppFileNaam}}.dll"]
