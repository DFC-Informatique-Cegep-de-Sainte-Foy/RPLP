#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RPLP.COHERENCECHECKER/RPLP.COHERENCECHECKER.csproj", "RPLP.COHERENCECHECKER/"]
COPY ["RPLP.DAL.SQL/RPLP.DAL.SQL.csproj", "RPLP.DAL.SQL/"]
COPY ["RPLP.DAL.DTO/RPLP.DAL.DTO.csproj", "RPLP.DAL.DTO/"]
COPY ["RPLP.ENTITES/RPLP.ENTITES.csproj", "RPLP.ENTITES/"]
COPY ["RPLP.SERVICES/RPLP.SERVICES.csproj", "RPLP.SERVICES/"]
RUN dotnet restore "RPLP.COHERENCECHECKER/RPLP.COHERENCECHECKER.csproj"
COPY . .
WORKDIR "/src/RPLP.COHERENCECHECKER"
RUN dotnet build "RPLP.COHERENCECHECKER.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RPLP.COHERENCECHECKER.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RPLP.COHERENCECHECKER.dll"]