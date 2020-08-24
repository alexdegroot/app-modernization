ARG Project=WriteApi
ARG ProjectFile="./$Project.csproj"

# Get smallest SDK image
FROM mcr.microsoft.com/dotnet/core/sdk:5.0-alpine AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY $ProjectFile ./
RUN dotnet restore $ProjectFile

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out -r linux-musl-x64  --self-contained true $ProjectFile

# Get runtime image
FROM alpine:3

# Install dependencies
RUN apk add --no-cache libstdc++ libintl

EXPOSE 80
EXPOSE 443

# Copy files from buildenv
WORKDIR /app
COPY --from=build-env /app/out .

# Disable globalization to avoud having to install 'icu'
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1

ENTRYPOINT ./WriteApi --urls http://0.0.0.0:80