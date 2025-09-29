# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/successful/Reservation.SuccessfulMessages/Reservation.SuccessfulMessages.csproj", "src/successful/Reservation.SuccessfulMessages/"]
COPY ["src/Reservation.Common/Reservation.Common.csproj", "src/Reservation.Common/"]
COPY ["src/Reservation.Infrastructure/Reservation.Infrastructure.csproj", "src/Reservation.Infrastructure/"]
COPY ["src/Reservation.Initializer/Reservation.Initializer.csproj", "src/Reservation.Initializer/"]
COPY ["src/Reservation.Persistence/Reservation.Persistence.csproj", "src/Reservation.Persistence/"]
RUN dotnet restore "./src/successful/Reservation.SuccessfulMessages/Reservation.SuccessfulMessages.csproj"
COPY . .
WORKDIR "/src/src/successful/Reservation.SuccessfulMessages"
RUN dotnet build "./Reservation.SuccessfulMessages.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Reservation.SuccessfulMessages.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Reservation.SuccessfulMessages.dll"]