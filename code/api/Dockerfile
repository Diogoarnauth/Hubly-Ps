FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dev
WORKDIR /app

# Como o Dockerfile está em code/api, os projetos estão nas subpastas
COPY ["Hubly.Api/Hubly.Api.csproj", "Hubly.Api/"]
COPY ["Hubly.Domain/Hubly.Domain.csproj", "Hubly.Domain/"]
COPY ["Hubly.Services/Hubly.Services.csproj", "Hubly.Services/"]
COPY ["Hubly.Infrastructure/Hubly.Infrastructure.csproj", "Hubly.Infrastructure/"]

RUN dotnet restore "Hubly.Api/Hubly.Api.csproj"

EXPOSE 8080

ENTRYPOINT ["dotnet", "watch", "run", "--project", "Hubly.Api/Hubly.Api.csproj", "--urls", "http://0.0.0.0:8080"]