const database = require('../database')

async function getUsernameId(username) {
    const { rows, rowCount } = await database.query('SELECT id FROM users WHERE users.username = $1;', [username])

    if (rowCount === 0)
        throw "Username not found"

    return rows[0].id
}

async function getIdUsername(id) {
    const { rows, rowCount } = await database.query('SELECT username FROM users WHERE users.id = $1;', [id])

    if (rowCount === 0)
        throw "Username not found"

    return rows[0].username
}

module.exports = { getUsernameId, getIdUsername }