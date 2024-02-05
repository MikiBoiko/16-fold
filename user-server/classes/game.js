const database = require('../database')
const { spawn } = require('node:child_process');
const { getUsernameId, getIdUsername } = require('./user');

const GAMESERVER_DIR = process.env.GAMESERVER_DIR

// TODO : redo my own algorithm (lazy + idk if licensed) https://stackoverflow.com/questions/1349404/generate-random-string-characters-in-javascript
function generateKey(length) {
    let result = '';
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    const charactersLength = characters.length;
    let counter = 0;
    while (counter < length) {
        result += characters.charAt(Math.floor(Math.random() * charactersLength));
        counter += 1;
    }
    return result;
}

async function fetchTagGame(tag, username) {
    const userId = await getUsernameId(username)

    const { rows: gameRows, rowCount: gameRowCount } = await database.query('SELECT redId, redKey, blackId, blackKey FROM games WHERE tag = $1;', [tag])

    if (gameRowCount === 0) {
        throw "Game not found"
    }

    const game = gameRows[0]

    const key = userId === game.redid
        ? game.redkey
        : game.blackkey

    console.log(game)
    console.log(userId, game.redid, key)

    const { rows: serverRows, rowCount: serverRowCount } = await database.query('SELECT hostname, port FROM game_servers WHERE game_tag = $1;', [tag])

    if (serverRowCount === 0) {
        throw "Server not found"
    }

    const { hostname, port } = serverRows[0]

    return {
        key,
        hostname,
        port
    }
}

async function fetchGames(username) {
    const userId = await getUsernameId(username)
    const { rows } = await database.query('SELECT tag, redId, blackId FROM games WHERE redId = $1 OR blackId = $1;', [userId])

    let games = []
    for (let index = 0; index < rows.length; index++) {
        const row = rows[index]
        const { tag, redId, blackId } = row

        const rivalId = userId === redId ? blackId : redId
        const rivalUsername = await getIdUsername(rivalId)

        games.push({
            tag,
            rival: rivalUsername
        })
    }

    return games
}

async function createGame(format, redId, blackId) {
    const availableServer = (await database.query('SELECT id, hostname, port FROM game_servers WHERE available = true ORDER BY port ASC;')).rows[0]

    const { id, port } = availableServer

    // Generate tag
    let tag
    let check = false

    while (check === false) {
        tag = generateKey(16)
        console.log('tag:', tag)
        const { rowCount } = await database.query('SELECT * from games WHERE tag = $1', [tag])

        if (rowCount === 0) check = true
    }

    // Generate player keys
    const redKey = generateKey(16)
    console.log('redKey:', redKey)
    let blackKey
    check = false
    while (check === false) {
        blackKey = generateKey(16)
        console.log('blackKey:', blackKey)
        if (blackKey !== redKey) check = true
    }

    await database.query(
        "INSERT INTO games(tag, format, redId, redKey, blackId, blackKey) VALUES ($1, $2, $3, $4, $5, $6);",
        [tag, format, redId, redKey, blackId, blackKey]
    )

    await database.query(
        "UPDATE game_servers SET available = 'false', game_tag = $1 WHERE id = $2;",
        [tag, id]
    )

    const gameServer = spawn(
        "dotnet",
        ["run", "--port", port, "--redKey", redKey, "--blackKey", blackKey, "--format", format],
        { cwd: GAMESERVER_DIR }
    )

    gameServer.stdout.on('data', (chunk) => {
        console.log(`stdout: ${chunk}`)
    })

    gameServer.stderr.on('data', (data) => {
        console.error(`stderr: ${data}`)
    })

    gameServer.on('close', (code) => {
        console.log(`child process close all stdio with code ${code}`);
    });

    gameServer.on('exit', (code) => {
        console.log(`child process exited with code ${code}`);
    });

    return tag
}

module.exports = { createGame, fetchGames, fetchTagGame }