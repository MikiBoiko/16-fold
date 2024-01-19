/* TODO: SCHEMAS */
DROP SCHEMA IF EXISTS web CASCADE;
CREATE SCHEMA web;

ALTER DEFAULT PRIVILEGES GRANT ALL ON TABLES TO user_server;
GRANT INSERT, UPDATE, SELECT, DELETE
ON ALL TABLES IN SCHEMA web 
TO user_server;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA web TO user_server;

/* TABLES */
DROP TABLE IF EXISTS users;
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(16) NOT NULL,
    password VARCHAR(60) NOT NULL,
    elo NUMERIC(6, 2) NOT NULL,
    gameCount NUMERIC(6),
    email TEXT NOT NULL,
    connected BOOLEAN NOT NULL,
    UNIQUE (username)
);
GRANT USAGE, SELECT ON SEQUENCE users_id_seq TO user_server;

DROP TABLE IF EXISTS queues;
CREATE TABLE queues (
    id SERIAL PRIMARY KEY,
    userId SERIAL NOT NULL,
    format VARCHAR(3) NOT NULL
);
GRANT USAGE, SELECT ON SEQUENCE queues_id_seq TO user_server;

DROP TABLE IF EXISTS game_servers;
CREATE TABLE game_servers(
    id SERIAL PRIMARY KEY,
    available BOOLEAN NOT NULL,
    hostname TEXT NOT NULL,
    port NUMERIC(5) NOT NULL
);
GRANT USAGE, SELECT ON SEQUENCE game_servers_id_seq TO user_server;

DROP TABLE IF EXISTS games;
CREATE TABLE games (
    id SERIAL PRIMARY KEY,
    tag VARCHAR(16) NOT NULL,
    format VARCHAR(3) NOT NULL,
    redId SERIAL NOT NULL,
    redKey VARCHAR(16) NOT NULL,
    blackId SERIAL NOT NULL,
    blackKey VARCHAR(16) NOT NULL,
    server SERIAL NOT NULL,
    result NUMERIC(1),
    way NUMERIC(1),
    UNIQUE(tag),
    FOREIGN KEY (redId) REFERENCES users(id),
    FOREIGN KEY (blackId) REFERENCES users(id),
    FOREIGN KEY (server) REFERENCES game_servers(id)
);
GRANT USAGE, SELECT ON SEQUENCE games_id_seq TO user_server;

SELECT * FROM users;
SELECT * FROM games;
SELECT * FROM queues;