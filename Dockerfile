# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy csproj for restore
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish BookApi.csproj -c Release -o out

# Debug: List what we built
RUN ls -la out/

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copy only the published output from build stage
COPY --from=build /app/out/ ./

# Debug: List what we have
RUN ls -la && echo "--- Looking for BookApi.dll ---" && find . -name "*.dll"

# Set environment variables - Railway provides PORT
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080}
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

# Run the app (DLL should be directly in /app)
CMD ["dotnet", "BookApi.dll"]
