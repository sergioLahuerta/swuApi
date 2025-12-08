# #!/bin/bash

# set -x
# set -e

# # --- Define la ruta correcta de sqlcmd ---
# SQLCMD=/opt/mssql-tools18/bin/sqlcmd
# # NOTA: La variable DB_NAME no se usa en la espera

# # --- Inicia SQL Server en segundo plano ---
# /opt/mssql/bin/sqlservr &

# sleep 15
# # --- Espera a que SQL Server esté listo ---
# echo "Esperando a que SQL Server inicie..."
# # 🚨 CORRECCIÓN: Quitamos -d "$DB_NAME". Nos conectamos solo a la instancia principal.
# until $SQLCMD -S localhost -U "$DB_USER" -P "$DB_PASSWORD" -Q "SELECT 1" -C >/dev/null
# do
#   sleep 2
# done
# echo "SQL Server listo para conexiones."

# # --- Ejecuta script SQL inicial ---
# echo "Ejecutando script SQL inicial..."
# # 🚨 NOTA: El script db.sql DEBE comenzar con CREATE DATABASE $DB_NAME, 
# # ya que el script todavía no se ha conectado a ninguna base de datos específica.
# if $SQLCMD -S localhost -U "$DB_USER" -P "$DB_PASSWORD" -Q "SELECT 1" -C -i /usr/src/app/scripts/db.sql -b -o /usr/src/app/scripts/output.log; then
#     echo "Script SQL ejecutado correctamente."
# else
#     echo "❌❌❌❌❌❌ Error al ejecutar el script SQL ❌❌❌❌❌❌" >&2
# fi

# # --- Mantiene SQL Server en primer plano ---
# wait