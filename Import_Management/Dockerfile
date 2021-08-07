#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Import_Management/Import_Management.csproj", "Import_Management/"]
COPY ["Domain.Core/Domain.Core.csproj", "Domain.Core/"]
COPY ["Infra.Bus/Infra.Bus.csproj", "Infra.Bus/"]
RUN dotnet restore "Import_Management/Import_Management.csproj"
COPY . .
WORKDIR "/src/Import_Management"
RUN dotnet build "Import_Management.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Import_Management.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Import_Management.dll"]
