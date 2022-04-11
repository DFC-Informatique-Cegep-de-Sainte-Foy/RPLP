#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RPLP.MVC/RPLP.MVC.csproj", "RPLP.MVC/"]
RUN dotnet restore "RPLP.MVC/RPLP.MVC.csproj"
COPY . .
WORKDIR "/src/RPLP.MVC"
RUN dotnet build "RPLP.MVC.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RPLP.MVC.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RPLP.MVC.dll"]