FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /build
COPY ["src/StoreServices.Api.Author/StoreServices.Api.Author.csproj", "src/StoreServices.Api.Author/"]
RUN dotnet restore "src/StoreServices.Api.Author/StoreServices.Api.Author.csproj"
COPY . .
WORKDIR "/build/src/StoreServices.Api.Author"
RUN dotnet build "StoreServices.Api.Author.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StoreServices.Api.Author.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StoreServices.Api.Author.dll"]
