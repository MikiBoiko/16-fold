const { Pool } = require('pg');

const pool = new Pool();

async function query(query, queryValues = []) {
    const { rows, rowCount } = await pool.query(query, queryValues);
    return { rows, rowCount };
}

module.exports = { query };