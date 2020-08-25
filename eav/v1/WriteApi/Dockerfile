ARG Project=WriteApi

# Get smallest SDK image
FROM mcr.microsoft.com/dotnet/core/sdk:5.0-alpine AS build-env
WORKDIR /app

# Create project to look for
ARG Project
ENV ProjectFile="./$Project.csproj"

# Copy csproj and restore as distinct layers
COPY $ProjectFile ./
RUN dotnet restore $ProjectFile

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out -r linux-musl-x64  --self-contained true $ProjectFile

#  runtime image
FROM alpine:3

# Application to run
ARG Project
ENV APPTORUN=${Project}

# Install dependencies
RUN apk add --no-cache libstdc++ libintl icu

# Move TLS back one level to ensure that SQL Server likes it
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf

# Copy files from buildenv
WORKDIR /app
COPY --from=build-env /app/out .

# Webserver ports
EXPOSE 80
EXPOSE 443

# Enable globalization (which also requires icu)
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0

# Start application
ENTRYPOINT "./${APPTORUN}" --urls http://0.0.0.0:80