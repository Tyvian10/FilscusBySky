FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY FilscusBySky.Models/FilscusBySky.Models.csproj FilscusBySky.Models/
COPY FilscusBySky.Data/FilscusBySky.Data.csproj FilscusBySky.Data/
COPY FilscusBySky.BusinessLogic/FilscusBySky.BusinessLogic.csproj FilscusBySky.BusinessLogic/
COPY FilscusBySky.Web/FilscusBySky.Web.csproj FilscusBySky.Web/

RUN dotnet restore FilscusBySky.Web/FilscusBySky.Web.csproj

COPY . .

RUN dotnet publish FilscusBySky.Web/FilscusBySky.Web.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FilscusBySky.Web.dll"]