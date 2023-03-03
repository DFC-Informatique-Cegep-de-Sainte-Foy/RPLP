FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RPLP.JOURNALISATION/RPLP.JOURNALISATION.csproj", "RPLP.JOURNALISATION/"]

RUN dotnet restore "RPLP.JOURNALISATION/RPLP.JOURNALISATION.csproj"
COPY . .
WORKDIR "/src/RPLP.JOURNALISATION"
RUN dotnet build "RPLP.JOURNALISATION.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RPLP.JOURNALISATION.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RPLP.JOURNALISATION.dll"]