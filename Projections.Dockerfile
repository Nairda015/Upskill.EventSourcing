FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ./src ./src
RUN dotnet restore "src/Projections/Projections.csproj"

RUN dotnet build "src/Projections/Projections.csproj" -c Release -o /app/build

FROM build as publish
RUN dotnet publish "src/Projections/Projections.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Projections.dll"]
