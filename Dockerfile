
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY RaceCompetitionAPI/RaceCompetitionAPI.csproj RaceCompetitionAPI/
RUN dotnet restore RaceCompetitionAPI/RaceCompetitionAPI.csproj

COPY . .
WORKDIR /src/RaceCompetitionAPI
RUN dotnet publish RaceCompetitionAPI.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "RaceCompetitionAPI.dll"]
