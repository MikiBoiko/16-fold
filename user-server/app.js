const cors = {
    origin: 'http://localhost:8100', // Allow requests from any origin
    credentials: true // Allow authentication credentials
}

require('dotenv').config()
const HOSTNAME = process.env.HOSTNAME ?? 'localhost'
const PORT = process.env.PORT ?? 3001

const app = require('express')()
const server = require('http').createServer(app)

const { Server } = require('socket.io')
const io = new Server(server, { cors })

const { access } = require('./routes/access')
const { users } = require('./routes/users')
const { games } = require('./routes/games')

const database = require('./database')
const auth = require('./auth')


app.use(require('cors')(cors))

app.use('/access', access)
app.use('/users', users)
app.use('/games', games)

//const io = require('socket.io')(server, { cors: cors })
const gameApi = require('./classes/game')
const userApi = require('./classes/user')

io.on('connection', (socket) => {
    console.log('connection try')
    const token = socket.handshake.auth.token;
    console.log(token)

    const sendError = (error) => {
        socket.emit("error", error)
    }

    if (token === undefined) {
        socket.disconnect()
        sendError("No token for socket connection.")
        return
    }

    let user

    try {
        const decoded = auth.verifyPublicToken(token)

        user = decoded.data
    }
    catch (err) {
        console.error(err)
        socket.disconnect()
        sendError("Bad token.")
        return
    }

    database.query("UPDATE users SET connected = 'true' WHERE username = $1;", [user.username])
    console.log('user connected');

    socket.join(user.username)

    socket.on('send-challenge', (username) => {
        console.log(username + ' is challenged by ' + user.username)
        io.to(username).emit('new-challenge', { username: user.username, format: '5+5' })
    })

    socket.on('cancel-challenge', (index) => {
        socket.emit('challenge-cancelled', index)
    })

    async function acceptChallenge(username, format) {
        const thisUserId = await userApi.getUsernameId(user.username) 
        const otherUserId = await userApi.getUsernameId(username) 
    
        const result = await gameApi.createGame(format, thisUserId, otherUserId)

        return result
    }

    socket.on('fetch-games', () => {
        gameApi.fetchGames(user.username)
        .then((games) => {
            socket.emit('fetch-games', games)
        })
        .catch((error) => {
            sendError(error)
        })
    })

    socket.on('accept-challenge', (username, format) => {
        acceptChallenge(username, format).then((tag) => {
            console.log('game created', tag)

            socket.emit('new-game', {
                tag,
                rival: username
            })

            io.to(username).emit('new-game', {
                tag,
                rival: user.username
            })
        })
        .catch((error) => {
            sendError(error)
        })
    })

    socket.on('disconnect', () => {
        database.query("UPDATE users SET connected = 'false' WHERE username = $1;", [user.username])
        socket.leave(user.username)
        console.log('user disconnected');
    });
})

database.query(
    "UPDATE users SET connected = 'false';" +
    "CALL setUpGameServers();"
)
.then(() => {
    server.listen(PORT, HOSTNAME, (err) => {
        if (err)
            console.log(err)
        console.log(`Listening at ${HOSTNAME}:${PORT}`)
    })
})
.catch((err) => {
    console.error(err)
})