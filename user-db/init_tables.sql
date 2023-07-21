DROP TABLE users;
DROP TABLE games;

CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    username varchar(16) NOT NULL,
    password varchar(60) NOT NULL,
    elo NUMERIC(6, 2) NOT NULL
    email TEXT NOT NULL
);

CREATE TABLE games (
    id SERIAL PRIMARY KEY,
    red_id SERIAL NOT NULL,
    black_id SERIAL NOT NULL,
    status SMALLINT NOT NULL,
    game_sfgn TEXT,
    timestamp TIMESTAMPTZ NOT NULL,

    FOREIGN KEY (red_id) REFERENCES users(id),
    FOREIGN KEY (black_id) REFERENCES users(id),
);