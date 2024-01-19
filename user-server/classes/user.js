const database = require('../database')

async function getUsernameId(username) {
    const { rows, rowCount } = await database.query('SELECT id FROM users WHERE users.username = $1;', [username])

    console.log(rows, rowCount)

    if (rowCount === 0) {
        throw "Username not found"
    }

    console.log('username ' + username + ' has id of ', rows)

    return rows[0].id
}

module.exports = { getUsernameId }