#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MonitoringNetCore.csproj", "."]
RUN dotnet restore "./MonitoringNetCore.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "MonitoringNetCore.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MonitoringNetCore.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
EXPOSE 7009
EXPOSE 5273
WORKDIR /app
RUN apt-get update && apt-get install ffmpeg htop -y
COPY --from=publish /app/publish .
COPY ./mediamtx /mediamtx
COPY ./entrypoint.sh ./entrypoint.sh
EXPOSE 5273
ENTRYPOINT ["./entrypoint.sh"]
