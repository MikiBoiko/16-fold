const database = require('../database')
const { spawn } = require('node:child_process')

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

async function createGame(format, redId, blackId) {
    let host = `localhost`
    let port = 5000

    // Generate tag
    let tag
    let check = false

    while (check === false) {
        tag = generateKey(16)
        console.log('tag:', tag)
        const { rows, rowCount } = await database.query('SELECT * from games WHERE tag = $1', [tag])

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
        "INSERT INTO games(tag, format, redId, redKey, blackId, blackKey, host, port) VALUES ($1, $2, $3, $4, $5, $6, $7)",
        [tag, format, redId, redKey, blackId, blackKey, host, port]
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

    return {
        redKey,
        blackKey,
        tag
    }
}

module.exports = { createGame }