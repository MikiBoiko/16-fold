#!/bin/bash

# Create the database.
psql -c 'DROP DATABASE IF EXISTS fold_db;'
psql -c 'CREATE DATABASE fold_db;'

# Create the user.
psql -c 'DROP USER IF EXISTS user_server;'
psql -c "CREATE USER user_server WITH ENCRYPTED PASSWORD 'hiThere';"

# Grant privileges to the user.
psql -c 'GRANT CONNECT ON DATABASE fold_db TO user_server;'
psql -c 'GRANT ALL PRIVILEGES ON DATABASE fold_db TO user_server;'

# Create the tables over database with user.
psql -d fold_db -f init_tables.sql -a