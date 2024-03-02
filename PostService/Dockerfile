FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PostService.csproj", "./"]
RUN dotnet restore "PostService.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "PostService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PostService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PostService.dll"]