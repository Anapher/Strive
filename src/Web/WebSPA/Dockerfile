#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

ARG NODE_IMAGE=node:14.0
FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://0.0.0.0:80

FROM ${NODE_IMAGE} as node-build
WORKDIR /web
COPY Client/package.json .
COPY Client/yarn.lock .
RUN yarn install
COPY Client/ .
RUN yarn build

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS publish
WORKDIR /src
COPY WebSPA.csproj .
COPY language.props .
RUN dotnet restore "WebSPA.csproj"
COPY . .
COPY --from=node-build /web/build /src/Client/build
RUN dotnet publish "WebSPA.csproj" --no-restore -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ARG GITCOMMIT
ARG GITREF
ARG GITTIMESTAMP

ENV App__GitInfo__Commit=$GITCOMMIT
ENV App__GitInfo__Ref=$GITREF
ENV App__GitInfo__Timestamp=$GITTIMESTAMP

ENTRYPOINT ["dotnet", "WebSPA.dll"]