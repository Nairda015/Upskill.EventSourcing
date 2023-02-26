FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ./src ./src
RUN dotnet restore "src/Queries/Queries.csproj"

RUN dotnet build "src/Queries/Queries.csproj" -c Release -o /app/build

FROM build as publish
RUN dotnet publish "src/Queries/Queries.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Queries.dll"]
