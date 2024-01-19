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

const database = require('./database')
const auth = require('./auth')


app.use(require('cors')(cors))

app.use('/access', access)
app.use('/users', users)

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
    
        await gameApi.createGame(format, thisUserId, otherUserId)
    }

    socket.on('accept-challenge', (username, format) => {
        acceptChallenge(username, format).then((result) => {
            console.log('game created', result)

            
        })
    })

    socket.on('disconnect', () => {
        database.query("UPDATE users SET connected = 'false' WHERE username = $1;", [user.username])
        socket.leave(user.username)
        console.log('user disconnected');
    });
})

server.listen(PORT, HOSTNAME, (err) => {
    if (err)
        console.log(err)

    database.query("UPDATE users SET connected = 'false';") // TODO: manage all previously connected clients at start

    console.log(`Listening at ${HOSTNAME}:${PORT}`)
})