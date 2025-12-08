#!/bin/bash

# Obtener la ruta absoluta del directorio donde está el script
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# El archivo .env.example está un nivel arriba del script
ROOT_DIR="$(dirname "$SCRIPT_DIR")"

EXAMPLE_FILE="$ROOT_DIR/.env.example"
ENV_FILE="$ROOT_DIR/.env"

# Verificar si existe .env.example
if [ ! -f "$EXAMPLE_FILE" ]; then
  echo "Error: El archivo .env.example no se encuentra en: $EXAMPLE_FILE"
  exit 1
fi

echo "Copiando .env.example a .env..."
cp "$EXAMPLE_FILE" "$ENV_FILE"

if [ $? -ne 0 ]; then
  echo "Error al copiar .env.example."
  exit 1
fi

if [ $? -ne 0 ]; then
  echo "No se pudo eliminar .env.example."
  exit 1
fi

echo "Archivo .env copiado y .env.example eliminado."
echo "Recuerda rellenar las variables de entorno en el archivo .env antes de continuar."
echo "Configuración inicial completa."