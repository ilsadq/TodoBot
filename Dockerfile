﻿FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TodoBot.csproj", "./"]
RUN dotnet restore "TodoBot.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "TodoBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TodoBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TodoBot.dll"]