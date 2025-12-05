# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy csproj and NuGet config for restore
COPY *.csproj ./
COPY NuGet.Config ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copy only the published output (NOT global.json or other files)
COPY --from=build /app/out ./

# Set environment variables
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Railway sets PORT, but we listen on 8080 by default
EXPOSE 8080

# Run the app
CMD ["dotnet", "BookApi.dll"]
