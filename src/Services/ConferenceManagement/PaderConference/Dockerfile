#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Services/ConferenceManagement/PaderConference/PaderConference.csproj", "Services/ConferenceManagement/PaderConference/"]
COPY ["Services/ConferenceManagement/PaderConference.Core/PaderConference.Core.csproj", "Services/ConferenceManagement/PaderConference.Core/"]
COPY ["Libs/JsonPatchGenerator/JsonPatchGenerator.csproj", "Libs/JsonPatchGenerator/"]
COPY ["Services/ConferenceManagement/PaderConference.Infrastructure/PaderConference.Infrastructure.csproj", "Services/ConferenceManagement/PaderConference.Infrastructure/"]
RUN dotnet restore "Services/ConferenceManagement/PaderConference/PaderConference.csproj"
COPY . .
WORKDIR "/src/Services/ConferenceManagement/PaderConference"
RUN dotnet build "PaderConference.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PaderConference.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaderConference.dll"]