#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RPLP.API/RPLP.API.csproj", "RPLP.API/"]
COPY ["RPLP.DAL.SQL/RPLP.DAL.SQL.csproj", "RPLP.DAL.SQL/"]
COPY ["RPLP.DAL.DTO/RPLP.DAL.DTO.csproj", "RPLP.DAL.DTO/"]
COPY ["RPLP.ENTITES/RPLP.ENTITES.csproj", "RPLP.ENTITES/"]
COPY ["RPLP.SERVICES/RPLP.SERVICES.csproj", "RPLP.SERVICES/"]
RUN dotnet restore "RPLP.API/RPLP.API.csproj"
COPY . .
WORKDIR "/src/RPLP.API"
RUN dotnet build "RPLP.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RPLP.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RPLP.API.dll"]