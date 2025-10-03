FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated9.0 AS base
WORKDIR /home/site/wwwroot
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ProductManagement.csproj", "."]
RUN dotnet restore "ProductManagement.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "ProductManagement.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductManagement.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true