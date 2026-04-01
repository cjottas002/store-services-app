FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /build
COPY ["src/StoreServices.Api.Gateway/StoreServices.Api.Gateway.csproj", "src/StoreServices.Api.Gateway/"]
RUN dotnet restore "src/StoreServices.Api.Gateway/StoreServices.Api.Gateway.csproj"
COPY . .
WORKDIR "/build/src/StoreServices.Api.Gateway"
RUN dotnet build "StoreServices.Api.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StoreServices.Api.Gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StoreServices.Api.Gateway.dll"]
