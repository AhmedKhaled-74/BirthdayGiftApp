# Angular SPA build (production uses same-origin API, see environment.ts)
FROM node:20-bookworm AS web-build
WORKDIR /src/age-calculator-web
COPY age-calculator-web/package.json age-calculator-web/package-lock.json ./
RUN npm ci
COPY age-calculator-web/ ./
RUN npm run build

# .NET API build (Ubuntu Noble: .NET 10 has no Debian images)
FROM mcr.microsoft.com/dotnet/sdk:10.0-noble AS api-build
WORKDIR /src
COPY AgeCalculator.slnx ./
COPY AgeCalculator.Api/AgeCalculator.Api.csproj AgeCalculator.Api/
RUN dotnet restore AgeCalculator.Api/AgeCalculator.Api.csproj
COPY AgeCalculator.Api/ AgeCalculator.Api/
COPY --from=web-build /src/age-calculator-web/dist/age-calculator-web/browser/. ./AgeCalculator.Api/wwwroot/
RUN dotnet publish AgeCalculator.Api/AgeCalculator.Api.csproj -c Release -o /app/publish /p:SkipSpaBuild=true

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble AS runtime
WORKDIR /app
COPY --from=api-build /app/publish ./
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000
ENTRYPOINT ["dotnet", "AgeCalculator.Api.dll"]
