require('dotenv').config()
const PORT = process.env.PORT ?? 3001;

const express = require('express')
const app = express()

const { signup_router } = require('./routes/signup')

app.use('/sign-up', signup_router)

app.listen(PORT, (err) => {
    if(err)
    console.log(err)
    
    console.log(`Listening on port ${ PORT }`)
})