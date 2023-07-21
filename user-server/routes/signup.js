const express = require('express')
const signup_router = express.Router()

signup_router.use(express.urlencoded({ extended: true }))

signup_router.post('/login', (req, res) => {
    const username = req.body.username
    const password = req.body.password

    console.log('Login call:')
    console.log({
        username,
        password
    })

    res.sendStatus(200)
})

signup_router.post('/register', (req, res) => {
    const username = req.body.username
    const password = req.body.password
    const email = req.body.email
    
    console.log('Register call:')
    console.log({
        username,
        password,
        email
    })

    res.statusCode(200)
})

module.exports = { signup_router }