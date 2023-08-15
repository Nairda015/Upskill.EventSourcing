FROM public.ecr.aws/lambda/dotnet:7.2023.08.02.10 AS base

FROM mcr.microsoft.com/dotnet/sdk:7.0-bullseye-slim as build
COPY src /src
WORKDIR /src/Projections
RUN dotnet build "Projections.csproj" -o /app/build

FROM build AS publish
RUN dotnet publish "Projections.csproj" -c Release -o /app/publish

FROM base AS final
COPY --from=publish /app/publish ${LAMBDA_TASK_ROOT}
CMD [ "Projections::Projections.Function::FunctionHandler" ]
