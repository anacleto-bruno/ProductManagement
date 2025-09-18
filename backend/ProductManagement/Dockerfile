# Use the official .NET 9.0 runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Use the .NET 9.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ProductManagement.csproj", "./"]
RUN dotnet restore "ProductManagement.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "ProductManagement.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductManagement.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Install Azure Functions Core Tools
RUN apt-get update && \
    apt-get install -y wget curl && \
    curl -sL https://deb.nodesource.com/setup_18.x | bash - && \
    apt-get install -y nodejs && \
    npm install -g azure-functions-core-tools@4 --unsafe-perm true && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

ENTRYPOINT ["func", "start", "--host", "0.0.0.0", "--port", "80"]