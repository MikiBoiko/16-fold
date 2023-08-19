import { createContext } from "react"

const gameContext = createContext({
  connection: null, // signalr client
  viewColor: null, // "red" | "black"
  startedResponse: null,
  endedResponse: null,
  boardState: {
    cardStackStates: {},
  },
  playingColor: null, // "red" | "black"
  turn: null, // "red" | "black"
  turnCount: 0,
  players: {
    red: null,
    black: null,
  },
  lastTurnTimeStamp: null, // date
  messages: []
})

export default gameContext
