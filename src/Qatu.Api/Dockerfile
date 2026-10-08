FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source
COPY src/Qatu.Domain/Qatu.Domain.csproj src/Qatu.Domain/
COPY src/Qatu.Application/Qatu.Application.csproj src/Qatu.Application/
COPY src/Qatu.Infrastructure/Qatu.Infrastructure.csproj src/Qatu.Infrastructure/
COPY src/Qatu.Api/Qatu.Api.csproj src/Qatu.Api/
RUN dotnet restore src/Qatu.Api/Qatu.Api.csproj
COPY src/ src/
RUN dotnet publish src/Qatu.Api/Qatu.Api.csproj -c Release --no-restore -o /out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /out .
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Qatu.Api.dll"]
