FROM mcr.microsoft.com/mssql/server:2019-latest

USER root

RUN apt-get -y update  && apt-get install -y dos2unix

RUN mkdir -p /usr/src/app
WORKDIR /usr/src/app
COPY . ./

RUN dos2unix *
RUN chmod +x /usr/src/app/import-data.sh

USER mssql

ENTRYPOINT /bin/bash /usr/src/app/entrypoint.sh