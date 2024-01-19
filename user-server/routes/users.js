const express = require('express')
const users = express.Router()

const database = require('../database')

async function searchUser(username) {
    const { rows, rowCount } = await database.query("SELECT username, connected FROM users WHERE username LIKE '%' || $1 || '%' ORDER BY username LIMIT 10;", [username])

    console.log(rows, rowCount)

    return rows
}

users.get('/search', (req, res) => {
    const { username } = req.query

    searchUser(username)
        .then((response) => {
            res.json(response)
        })
        .catch((err) => {
            console.error(err)
            res.status(500).json({ error: 'Bad search.' })
        })
})

async function profileOf(username) {
    const { rows, rowCount } = await database.query("SELECT username, elo, gameCount, connected FROM users WHERE username = $1;", [username])

    console.log(rows, rowCount)

    if (rowCount !== 1)
        throw "User not found."

    return rows[0]
}

users.get('/profile', (req, res) => {
    const { username } = req.query

    profileOf(username)
        .then((response) => {
            res.json(response)
        })
        .catch((err) => {
            console.error(err)
            res.status(500).json({ error: 'Bad profile.' })
        })
})


module.exports = { users }