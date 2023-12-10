#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/TaskCBT/TaskCBT.csproj", "src/TaskCBT/"]
COPY ["src/TaskCBT.Application/TaskCBT.Application.csproj", "src/TaskCBT.Application/"]
COPY ["src/TaskCBT.Domain/TaskCBT.Domain.csproj", "src/TaskCBT.Domain/"]
COPY ["src/TaskCBT.Infrastructure/TaskCBT.Infrastructure.csproj", "src/TaskCBT.Infrastructure/"]
RUN dotnet restore "./src/TaskCBT/./TaskCBT.csproj"
COPY . .
WORKDIR "/src/src/TaskCBT"
RUN dotnet build "./TaskCBT.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TaskCBT.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskCBT.dll"]