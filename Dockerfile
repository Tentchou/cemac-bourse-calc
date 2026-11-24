# Image de production : l'API ASP.NET Core sert aussi le site React compilé (une seule adresse).
# Construite par Cloud Build (gcloud run deploy --source .) : Docker n'est pas nécessaire en local.
# Versions épinglées, publiées depuis plus de deux semaines.

# 1. Le site React
FROM node:22.23.4-alpine AS front
WORKDIR /front
COPY front/package.json front/package-lock.json ./
RUN npm ci --no-audit --no-fund
COPY front/ ./
RUN npm run build

# 2. L'API (les .csproj d'abord : la restauration NuGet reste en cache tant qu'ils ne changent pas)
FROM mcr.microsoft.com/dotnet/sdk:10.0.404 AS api
WORKDIR /src
COPY src/CemacBourseCalc.Domain/CemacBourseCalc.Domain.csproj src/CemacBourseCalc.Domain/
COPY src/CemacBourseCalc.Application/CemacBourseCalc.Application.csproj src/CemacBourseCalc.Application/
COPY src/CemacBourseCalc.Infrastructure/CemacBourseCalc.Infrastructure.csproj src/CemacBourseCalc.Infrastructure/
COPY src/CemacBourseCalc.Api/CemacBourseCalc.Api.csproj src/CemacBourseCalc.Api/
RUN dotnet restore src/CemacBourseCalc.Api/CemacBourseCalc.Api.csproj
COPY src/ src/
RUN dotnet publish src/CemacBourseCalc.Api/CemacBourseCalc.Api.csproj -c Release -o /app --no-restore

# 3. L'image finale : le runtime seul, sans SDK ni Node, exécutée sans droits root
FROM mcr.microsoft.com/dotnet/aspnet:10.0.15
WORKDIR /app
COPY --from=api /app ./
COPY --from=front /front/dist ./wwwroot
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_gcServer=0
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "CemacBourseCalc.Api.dll"]
