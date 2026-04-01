FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /build
COPY ["src/StoreServices.Api.Book/StoreServices.Api.Book.csproj", "src/StoreServices.Api.Book/"]
RUN dotnet restore "src/StoreServices.Api.Book/StoreServices.Api.Book.csproj"
COPY . .
WORKDIR "/build/src/StoreServices.Api.Book"
RUN dotnet build "StoreServices.Api.Book.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StoreServices.Api.Book.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StoreServices.Api.Book.dll"]
