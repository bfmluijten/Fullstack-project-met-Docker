FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore "./Server.csproj"

# Run publish (do not skip restore so assets are resolved during publish)
RUN dotnet publish "./Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
# Copy published files from build stage
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Server.dll"]
