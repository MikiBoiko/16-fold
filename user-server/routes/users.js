const express = require('express')
const users = express.Router()

const database = require('../database')

async function searchUser(username) {
    const { rows, rowCount} = await database.query(username) 
}

users.post('/search', (req, res) => {

})

module.exports = { users }