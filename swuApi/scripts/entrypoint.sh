#!/bin/bash
# exec > /var/opt/mssql/log/entrypoint_debug.log 2>&1
set -x
set -e # ⬅️ Añadir para salir si un comando falla

# Define la ruta correcta de sqlcmd (adaptada a la versión 2019/2022)
SQLCMD=/opt/mssql-tools/bin/sqlcmd

# Inicia SQL Server en segundo plano
/opt/mssql/bin/sqlservr &

# Espera a que SQL Server esté listo
echo "Esperando a que SQL Server inicie..."
# 🚨 Usamos DB_PASSWORD (asumimos que env_file la inyecta)
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

# Mantén SQL Server en primer plano
wait