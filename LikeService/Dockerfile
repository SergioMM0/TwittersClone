
FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["LikeService.csproj", "./"]
RUN dotnet restore "LikeService.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "LikeService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LikeService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LikeService.dll"]