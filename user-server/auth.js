const jwt = require('jsonwebtoken')

const PUBLIC_SECRET = process.env.PUBLIC_SECRET
const PRIVATE_SECRET = process.env.PRIVATE_SECRET

/* SIGN */
const signToken = (user, secret) => {
    return jwt.sign({
        data: user
    }, secret)
}

const signPrivateToken = (user) => {
    const token = signToken(user, PRIVATE_SECRET)
    return token
}

const signPublicToken = (user) => {
    const token = signToken(user, PUBLIC_SECRET)
    return token
}

/* VERIFY */
function verifyToken(token, secret) {
    var decoded = jwt.verify(token, secret)
    return decoded
}

function verifyPrivateToken(token) {
    const decoded = verifyToken(token, PRIVATE_SECRET)
    return decoded
}

function verifyPublicToken(token) {
    const decoded = verifyToken(token, PUBLIC_SECRET)
    return decoded
}

function verifyPublicTokenMiddleware(req, res, next) {
    try {
        const token = req.headers.auth.split(' ')[1]
        req.decodedToken = verifyPublicToken(token)
        next()
    }
    catch (error) {
        res.status(500).json({ error })
    }
}

module.exports = { signPrivateToken, signPublicToken, verifyPrivateToken, verifyPublicToken, verifyPublicTokenMiddleware }