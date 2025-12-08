SWU Api README.md || Pasos a seguir: en GitBash
# Dentro del repositorio: En la carpeta swuApi: por GitBash
cd swuApi
# Ejecutamos script setup.sh para crear el .env
./scripts/setup.sh

code .

# En el archivo .env insertar los valores de las variables pasadas por el comentario privado de la tarea de este trabajo.

**Abrir la aplicación Docker Desktop**

# Crear los contenedores con la db y la api
**docker-compose up --build -d**

# Para ver el Swagger:
http://localhost:8309/swagger/index.html

# Hay algunas funcionalidades implementadas correctamente en la web, para verlas:
# En la raiz del repositorio ->
cd swuAppWeb

code .

Darle a la función **GoLive** (en caso de no tenerla, descargar extensión **Live Server**).
