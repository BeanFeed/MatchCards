FROM node:20 AS frontend
WORKDIR /src
COPY Frontend/package.json .
RUN npm install --legacy-peer-deps
COPY Frontend/. .
RUN npm run build

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MatchCards/MatchCards.csproj", "MatchCards/"]
RUN dotnet restore "MatchCards/MatchCards.csproj"
COPY . .
COPY --from=frontend /src/dist /src/MatchCards/wwwroot/
WORKDIR "/src/MatchCards"
RUN dotnet build "./MatchCards.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MatchCards.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MatchCards.dll"]
