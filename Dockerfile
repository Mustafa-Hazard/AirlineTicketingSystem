FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["AirlineTicketingSystem.csproj", "."]
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install libgdiplus for PDF generation (QuestPDF)
RUN apt-get update && apt-get install -y libgdiplus && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# Railway assigns PORT env variable
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}
EXPOSE 8080

ENTRYPOINT ["dotnet", "AirlineTicketingSystem.dll"]
