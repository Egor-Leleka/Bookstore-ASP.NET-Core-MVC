#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BookstoreWeb/BookstoreWeb.csproj", "BookstoreWeb/"]
COPY ["Bookstore.DataAccess/Bookstore.DataAccess.csproj", "Bookstore.DataAccess/"]
COPY ["Bookstore.Models/Bookstore.Models.csproj", "Bookstore.Models/"]
COPY ["Bookstore.Utility/Bookstore.Utility.csproj", "Bookstore.Utility/"]
RUN dotnet restore "./BookstoreWeb/./BookstoreWeb.csproj"
COPY . .
WORKDIR "/src/BookstoreWeb"
RUN dotnet build "./BookstoreWeb.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BookstoreWeb.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookstoreWeb.dll"]