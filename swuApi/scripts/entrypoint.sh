#!/bin/bash

set -x
set -e

SQLCMD=/opt/mssql-tools/bin/sqlcmd

/opt/mssql/bin/sqlservr &

# Espera a que SQL Server esté listo
echo "Esperando a que SQL Server inicie..."
# Con varuables del .env
until $SQLCMD -S localhost -U "$DB_USER" -P "$DB_PASSWORD" -Q "SELECT 1" -C &>/dev/null
do
sleep 2
done
echo "SQL Server listo para conexiones."

# Ejecuta script de creación de DB
echo "Ejecutando script SQL inicial..."
if $SQLCMD -S localhost -U "$DB_USER" -P "$DB_PASSWORD" -i /scripts/db.sql; then
    echo "Script SQL ejecutado correctamente."
else
    echo "❌❌❌ Error al ejecutar el script SQL ❌❌❌" >&2
fi

# Mantiene SQL Server en primer plano
wait