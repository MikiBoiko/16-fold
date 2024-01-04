const express = require('express')
const access = express.Router()

const auth = require('../auth')

const bcrypt = require('bcrypt')
const SALT = 10

const database = require('../database')

access.use(express.urlencoded({ extended: true }))

access.post('/', (req, res) => {
    const token = req.body.token

    if (token === undefined) {
        res.status(500).json('No token.')
    }

    try {
        const user = auth.verifyPrivateToken(token)

        console.log('DECODE:')
        console.log(user)

        res.json({
            user: user,
            publicToken: auth.signPublicToken(user)
        })
    } catch (error) {
        console.error(error)
        res.status(500).json('Bad token.')
    }
})

async function onLogin(username, password) {
    const userQueryResult = await database.query('SELECT * FROM users WHERE users.username = $1', [username])

    const { rows, rowCount } = userQueryResult

    console.log(userQueryResult)

    if (rowCount === 0) return false

    const user = rows[0]
    console.log(user)
    const hashedPassword = user.password

    const isMatch = await bcrypt.compare(password, hashedPassword)

    if (!isMatch)
        throw 'Wrong user credentials.'

    console.log(auth.signPrivateToken({ username }))

    return {
        auth: auth.signPrivateToken({ username })
    }
}

access.post('/login', (req, res) => {
    const username = req.body.username
    const password = req.body.password

    console.log('Login call:')
    console.log({
        username,
        password
    })

    onLogin(username, password)
        .then((response) => {
            res.json(response)
        })
        .catch((error) => { res.status(500).json(error) })
})

async function onRegister(username, password, email) {
    const userQueryResult = await database.query('SELECT * FROM users WHERE users.username = $1', [username])

    const { rows, rowCount } = userQueryResult

    console.log(userQueryResult)

    if (rowCount !== 0)
        throw 'User already exists.'
    else if (username.length < 1 || username.length > 16)
        throw 'Weird username.'
    else if (password.length < 8 || password.length > 72)
        throw 'Weird password.'

    const hashedPassword = await bcrypt.hash(password, SALT)
    console.log(hashedPassword.length)

    await database.query('INSERT INTO users(username, password, elo, gameCount, email) VALUES ($1, $2, 1500, 0, $3)', [username, hashedPassword, email])

    return {
        auth: auth.signPrivateToken({ username })
    }
}

access.post('/register', (req, res) => {
    const username = req.body.username
    const password = req.body.password
    const email = req.body.email

    console.log('Register call:')
    console.log({
        username,
        password,
        email
    })

    onRegister(username, password, email)
        .then((response) => {
            res.json(response)
        })
        .catch((error) => { res.status(500).json(error) })
})

module.exports = { access }