const tilePositions = [
  "a1",
  "b1",
  "c1",
  "d1",
  "a2",
  "b2",
  "c2",
  "d2",
  "a3",
  "b3",
  "c3",
  "d3",
  "a4",
  "b4",
  "c4",
  "d4",
  "a5",
  "b5",
  "c5",
  "d5",
  "a6",
  "b6",
  "c6",
  "d6",
  "a7",
  "b7",
  "c7",
  "d7",
]

const startingCardStacksState = {
  a1: null,
  b1: null,
  c1: null,
  d1: null,
  a3: null,
  b3: null,
  c3: null,
  d3: null,
  a5: null,
  b5: null,
  c5: null,
  d5: null,
  a7: null,
  b7: null,
  c7: null,
  d7: null,
}

function adjacentTo(a, b) {
  if (a === b) return false

  const af = tilePositions.indexOf(a),
    bf = tilePositions.indexOf(b)

  const xa = af % 4,
    ya = Math.floor(af / 4)

  const xb = bf % 4,
    yb = Math.floor(bf / 4)

  return Math.abs(xa - xb) <= 1 && Math.abs(ya - yb) <= 1
}

const values = [
  "Jk",
  "2",
  "3",
  "4",
  "5",
  "6",
  "7",
  "8",
  "9",
  "10",
  "J",
  "Q",
  "K",
  "A",
]
const symbols = {
  red: ["♥", "♦"],
  black: ["♣", "♠"],
}

const colors = ["red", "black", "none", "both"]

const valueToCard = (value, color) => {
  return values[Math.floor(value / 2)] + symbols[color][value % 2]
}

const reasons = ["AGREED", "PASSING", "MATERIAL", "TIME", "REPORT", "ILLEGAL"]

export {
  tilePositions,
  startingCardStacksState,
  adjacentTo,
  colors,
  valueToCard,
  reasons,
}
