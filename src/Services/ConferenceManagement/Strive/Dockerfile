#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://0.0.0.0:80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Services/ConferenceManagement/Strive/Strive.csproj", "Services/ConferenceManagement/Strive/"]
COPY ["Services/ConferenceManagement/Strive.Core/Strive.Core.csproj", "Services/ConferenceManagement/Strive.Core/"]
COPY ["Libs/JsonPatchGenerator/JsonPatchGenerator.csproj", "Libs/JsonPatchGenerator/"]
COPY ["Services/ConferenceManagement/Strive.Infrastructure/Strive.Infrastructure.csproj", "Services/ConferenceManagement/Strive.Infrastructure/"]
RUN dotnet restore "Services/ConferenceManagement/Strive/Strive.csproj"
COPY ["Services/ConferenceManagement/", "Services/ConferenceManagement/"]
COPY ["Libs/", "Libs/"]
WORKDIR "/src/Services/ConferenceManagement/Strive"

FROM build AS publish
RUN dotnet publish "Strive.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Strive.dll"]