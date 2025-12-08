# Setup local db (Sql server) | Antiguo comando para levantar la DB
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=${password}" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2019-CU21-ubuntu-20.04

# Comandos para levantar los contenedores | Docker
docker compose up --build -d

# Para subir la Imagen a DockerHub
docker login
docker-compose up --build -d
docker images
docker tag imagen-Id sergiolala/swuapi-sv:1.0.0
docker push sergiolala/swuapi-sv:1.0.0

# Para descargar la imagen
docker pull sergiolala/swuapi-sv:1.0.0