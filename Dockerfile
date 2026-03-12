FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY SwinStudy.Api/SwinStudy.Api.csproj SwinStudy.Api/
RUN dotnet restore SwinStudy.Api/SwinStudy.Api.csproj

COPY . .
RUN dotnet publish SwinStudy.Api/SwinStudy.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "SwinStudy.Api.dll"]