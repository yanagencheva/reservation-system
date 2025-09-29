# Stage 1: Base image (runtime)
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

# Optional: Create a non-root user
ARG APP_UID=1000
RUN useradd -u $APP_UID appuser
USER appuser

# Stage 2: Build image (SDK)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy all source files
COPY src ./src

# Restore all projects
WORKDIR /src/src/ReservationRestorantTable
RUN dotnet restore "ReservationRestorantTable.csproj"

# Build the project
RUN dotnet build "ReservationRestorantTable.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 3: Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ReservationRestorantTable.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4: Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReservationRestorantTable.dll"]
