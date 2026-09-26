FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/RestauranteApi/RestauranteApi.csproj", "src/RestauranteApi/"]
RUN dotnet restore "src/RestauranteApi/RestauranteApi.csproj"

COPY . .
WORKDIR "/src/src/RestauranteApi"
RUN dotnet publish "RestauranteApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 5000

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RestauranteApi.dll"]
