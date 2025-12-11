#!/bin/bash

# Obtener la ruta del archivo, se ejecuta en la raiz del proyecto
EXAMPLE_FILE=".env.example"
ENV_FILE=".env"

# Verificar si existe .env.example
if [ ! -f "$EXAMPLE_FILE" ]; then
  echo "Error: El archivo .env.example no se encuentra."
  exit 1
fi

# Copiar y crear .env
echo "Copiando $EXAMPLE_FILE a $ENV_FILE..."
cp "$EXAMPLE_FILE" "$ENV_FILE"

if [ $? -ne 0 ]; then
  echo "Error al copiar el archivo."
  exit 1
fi

echo "Archivo .env creado. Por favor, abre el archivo .env"
echo "y rellena los valores sensibles antes de continuar."