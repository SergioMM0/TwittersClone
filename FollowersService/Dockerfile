FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["FollowersService.csproj", "./"]
RUN dotnet restore "FollowersService.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "FollowersService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FollowersService.csproj" -c Release -o /app/publish --no-self-contained

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FollowersService.dll"]
