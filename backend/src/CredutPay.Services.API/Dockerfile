#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/CredutPay.Services.API/CredutPay.Services.API.csproj", "src/CredutPay.Services.API/"]
COPY ["src/CredutPay.Application/CredutPay.Application.csproj", "src/CredutPay.Application/"]
COPY ["src/CredutPay.Domain/CredutPay.Domain.csproj", "src/CredutPay.Domain/"]
COPY ["src/CredutPay.Domain.Core/CredutPay.Domain.Core.csproj", "src/CredutPay.Domain.Core/"]
COPY ["src/CredutPay.CrossCutting.Identity/CredutPay.CrossCutting.Identity.csproj", "src/CredutPay.CrossCutting.Identity/"]
COPY ["src/CredutPay.CrossCutting.IoC/CredutPay.CrossCutting.IoC.csproj", "src/CredutPay.CrossCutting.IoC/"]
COPY ["src/CredutPay.CrossCutting.Bus/CredutPay.CrossCutting.Bus.csproj", "src/CredutPay.CrossCutting.Bus/"]
COPY ["src/CredutPay.Infra.Data/CredutPay.Infra.Data.csproj", "src/CredutPay.Infra.Data/"]
RUN dotnet restore "./src/CredutPay.Services.API/./CredutPay.Services.API.csproj"
COPY . .
WORKDIR "/src/src/CredutPay.Services.API"
RUN dotnet build "./CredutPay.Services.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CredutPay.Services.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CredutPay.Services.API.dll"]