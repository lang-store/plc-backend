## BUILD
ARG BUILD_REGISTRY
FROM ${BUILD_REGISTRY}/dotnet/core/sdk:3.1 AS builder
ARG NEXUS_NUGET=https://nexus.ftc.ru/repository/nuget-group/

WORKDIR /app

COPY . ./
COPY ca/ /usr/local/share/ca-certificates/

RUN update-ca-certificates
RUN dotnet publish --source ${NEXUS_NUGET} -c Release -o out -r debian-x64


## RUNTIME
FROM ${BUILD_REGISTRY}/dotnet/core/runtime-deps:3.1 AS runtime
ENV SERVER_PORT=5003
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://0.0.0.0:${SERVER_PORT}


FROM ${BUILD_REGISTRY}/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=builder /app/out .
EXPOSE ${SERVER_PORT}

ENTRYPOINT ["dotnet", "registry.dll"]
