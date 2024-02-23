# Use the .NET Core SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy the project files and restore dependencies
COPY AuthService.csproj ./
RUN dotnet restore

# Copy the remaining files and build the application
COPY . ./
RUN dotnet publish -c Release -o out

# Use a smaller runtime image to run the application
FROM mcr.microsoft.com/dotnet/runtime:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Set the entry point for the application
ENTRYPOINT ["dotnet", "AuthService.dll"]