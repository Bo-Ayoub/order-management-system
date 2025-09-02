# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Build SDK image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Set Linux-friendly NuGet cache
ENV NUGET_PACKAGES=/root/.nuget/packages

# Copy solution and project files
COPY ["src/OrderManagement.API/OrderManagement.API.csproj", "src/OrderManagement.API/"]
COPY ["src/OrderManagement.Application/OrderManagement.Application.csproj", "src/OrderManagement.Application/"]
COPY ["src/OrderManagement.Infrastructure/OrderManagement.Infrastructure.csproj", "src/OrderManagement.Infrastructure/"]
COPY ["src/OrderManagement.Domain/OrderManagement.Domain.csproj", "src/OrderManagement.Domain/"]



# Restore all dependencies from solution
RUN dotnet restore "src/OrderManagement.API/OrderManagement.API.csproj"

# Copy all source code
COPY . .

# Build & publish in one step
WORKDIR "/src/src/OrderManagement.API"
RUN dotnet publish "OrderManagement.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app

# Create logs directory
RUN mkdir -p /app/logs

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published output
COPY --from=build /app/publish .

# Create non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "OrderManagement.API.dll"]
