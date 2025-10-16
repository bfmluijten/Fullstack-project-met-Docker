FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Server.csproj", "WebApp/"]
#COPY ["{{Eventueel locaties van andere dll projecten}}.csproj", "{{Locatie die je gebruikt in je WebApp}}/"]
RUN dotnet restore "WebApp/Server.csproj"
WORKDIR "/src/WebApp"
COPY . .
RUN dotnet publish "Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Server.dll"]
