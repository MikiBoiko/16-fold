require('dotenv').config()
const HOSTNAME = process.env.HOSTNAME ?? 'localhost'
const PORT = process.env.PORT ?? 3001

const app = require('express')()
const server = require('http').createServer(app)

const { access } = require('./routes/access')
const { users } = require('./routes/users')

app.use(require('cors')({ origin: 'http://localhost:8100'}))

app.use('/access', access)
app.use('/users', users)

const io = require('socket.io')(server, {
    cors: {
        origin: 'http://localhost:8100' // TODO
    }
})

io.on('connection', (socket) => {

})

app.listen(PORT, HOSTNAME, (err) => {
    if (err)
        console.log(err)

    console.log(`Listening at ${HOSTNAME}:${PORT}`)
})