#!/bin/bash

set -x
set -e

# Define la ruta correcta de sqlcmd (adaptada a la versión 2019/2022)
SQLCMD=/opt/mssql-tools/bin/sqlcmd

# Inicia SQL Server en segundo plano
/opt/mssql/bin/sqlservr &

# Espera a que SQL Server esté listo
echo "Esperando a que SQL Server inicie..."

# 🚨 Usamos DB_PASSWORD (asumimos que env_file la inyecta)
until $SQLCMD -S localhost -U "SA" -P "$SA_PASSWORD" -Q "SELECT 1" -C &>/dev/null
do
    sleep 2
done
echo "SQL Server listo para conexiones."

# Ejecuta script de creación de DB
echo "Ejecutando script SQL inicial..."
if [ ! -d "/var/opt/mssql/data" ]; then
    echo "INICIALIZACIÓN: El volumen está vacío. Ejecutando scripts de creación y datos iniciales..."
    
    if $SQLCMD -S localhost -U "SA" -P "$SA_PASSWORD" -i /scripts/db.sql; then
        echo "¡¡¡¡¡¡¡¡¡¡¡¡¡ Script SQL de inicialización ejecutado correctamente !!!!!!!!!!!!!!!"
    else
        echo "------------- Error al ejecutar el script SQL de inicialización --------------" >&2
        exit 1 # Detener el contenedor si falla la inicialización
    fi
    
else
    # 5. Si el directorio de datos existe, los datos persisten.
    echo "REINICIO: Datos de DB ya existentes en el volumen. Omitiendo scripts de inicialización."
fi

# Mantén SQL Server en primer plano
wait