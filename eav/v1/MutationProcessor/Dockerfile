ARG Project=MutationProcessor

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
RUN apk add --no-cache libstdc++ libintl

# Copy files from buildenv
WORKDIR /app
COPY --from=build-env /app/out .

# Disable globalization to avoud having to install 'icu'
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1

# Ensure that the code knows it runs in a container
ENV DOTNET_RUNNING_IN_CONTAINER=1

# Start application
ENTRYPOINT "./${APPTORUN}"