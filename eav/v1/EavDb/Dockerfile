FROM mcr.microsoft.com/mssql/server:2019-latest

USER root

RUN mkdir -p /usr/src/app
WORKDIR /usr/src/app
COPY . ./

RUN chmod +x /usr/src/app/import-data.sh

USER mssql

ENTRYPOINT /bin/bash /usr/src/app/entrypoint.sh