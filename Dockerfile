FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src

# Copy all
COPY *.sln ./
COPY Application/*.csproj ./Application/
COPY CrossCutting/*.csproj ./CrossCutting/
COPY CrossCutting.IoC/*.csproj ./CrossCutting.IoC/
COPY Domain/*.csproj ./Domain/
COPY Domain.Shared/*.csproj ./Domain.Shared/
COPY Infra.Data/*.csproj ./Infra.Data/
COPY XUnitTest/*.csproj ./XUnitTest/
COPY CallibrationTest/*.csproj ./CallibrationTest/
COPY Api/*.csproj ./Api/

# Restore all projects
RUN dotnet restore
COPY . .

# build each project
WORKDIR /src/Application
RUN dotnet build -c Release -o /app/build

WORKDIR /src/CrossCutting
RUN dotnet build -c Release -o /app/build

WORKDIR /src/CrossCutting.IoC
RUN dotnet build -c Release -o /app/build

WORKDIR /src/Domain
RUN dotnet build -c Release -o /app/build

WORKDIR /src/Domain.Shared
RUN dotnet build -c Release -o /app/build

WORKDIR /src/Infra.Data
RUN dotnet build -c Release -o /app/build

WORKDIR /src/Api
RUN dotnet build -c Release -o /app/build

# publish artifacts
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS http://*:$PORT
ENV ASPNETCORE_ENVIRONMENT DYNO
ENTRYPOINT ["dotnet", "Api.dll"]