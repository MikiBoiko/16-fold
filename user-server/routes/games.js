const games = require('express').Router()

const gamesApi = require('../classes/game')

const { verifyPublicTokenMiddleware } = require('../auth')

games.use(verifyPublicTokenMiddleware)

games.get('/fetch', (req, res) => {
    console.log('fetching')

    const username = req.decodedToken.data.username
    const tag = req.query.tag

    console.log(username, tag)

    gamesApi.fetchTagGame(tag, username)
        .then((response) => {
            console.log(response)
            res.json(response)
        })
        .catch((error) => {
            console.error(error)
            res.status(500).json(error)
        })
})

module.exports = { games }