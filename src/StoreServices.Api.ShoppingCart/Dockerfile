FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /build
COPY ["src/StoreServices.Api.ShoppingCart/StoreServices.Api.ShoppingCart.csproj", "src/StoreServices.Api.ShoppingCart/"]
RUN dotnet restore "src/StoreServices.Api.ShoppingCart/StoreServices.Api.ShoppingCart.csproj"
COPY . .
WORKDIR "/build/src/StoreServices.Api.ShoppingCart"
RUN dotnet build "StoreServices.Api.ShoppingCart.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StoreServices.Api.ShoppingCart.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StoreServices.Api.ShoppingCart.dll"]
