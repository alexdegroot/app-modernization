# app-modernization

## Compose docker containers
To compose and start Docker containers on your local machine (in the background), open a Powershell window and enter the following command:
docker-compose -f .\docker-compose.yml up -d --build

To stop the containers, enter the following command:
docker-compose down

## Run container for specific application with rebuild
docker-compose up --build <application-name>
Example:
docker-compose up --build readapi

Any dependencies defined in a 'depends_on' section of the application (in the docker-compose.yml file) will be resolved, i.e. the containers that the container depends on are also started.
The --build argument forces a rebuild of the container.
