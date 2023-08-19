import { useState, useEffect, useCallback } from "react"
import { DefaultHttpClient, HubConnectionBuilder } from "@microsoft/signalr"
import Board from "./game/Board"
import Chat from "./game/Chat"
import Profile from "./game/Profile"
import gameContext from "../context/gameContext"
import {
  colors,
  valueToCard,
  startingCardStacksState,
  reasons,
} from "./game/misc"
import "./Game.css"

class GameHttpClient extends DefaultHttpClient {
  send(request) {
    request.headers = { ...request.headers, token: "Game Test Token" }
    return super.send(request)
  }
}

const Game = (props) => {
  const { url } = props

  //#region Game context
  const [connection, setConnection] = useState(null)
  const [viewColor, setColorView] = useState("none")
  const [startedResponse, setStartedResponse] = useState(null)
  const [endedResponse, setEndedResponse] = useState(null)
  const [boardState, setBoardState] = useState({
    cardStackStates: startingCardStacksState,
  })
  const [playingColor, setPlayingColor] = useState("none")
  const [turn, setTurn] = useState(null)
  const [turnCount, setTurnCount] = useState(0)
  const [redPlayer, setRedPlayer] = useState(null)
  const [blackPlayer, setBlackPlayer] = useState(null)
  const [lastTurnTimeStamp, setLastTurnTimeStamp] = useState(null)
  const [messages, setMessages] = useState([])
  //#endregion

  //#region Class callbacks
  const nextTurn = useCallback(
    (timestamp) => {
      console.log(turn)
      setTurn(turn === "red" ? "black" : "red")
      setTurnCount(turnCount + 1)
      setLastTurnTimeStamp(timestamp)
    },
    [turn, turnCount]
  )

  //#region Chat
  const onRecieveUserMessage = useCallback(
    (data) => {
      // https://react.dev/learn/updating-arrays-in-state
      setMessages([...messages, { data, type: "user" }])
    },
    [messages, setMessages]
  )

  const onRecieveGameMessage = useCallback(
    (data) => {
      // https://react.dev/learn/updating-arrays-in-state
      setMessages([...messages, { data, type: "game" }])
    },
    [messages, setMessages]
  )
  //#endregion
  //#endregion

  //#region Effects
  useEffect(() => {
    //#region Open connection
    const connection = new HubConnectionBuilder()
      .withUrl(`${url}/game`, { httpClient: new GameHttpClient() })
      .withAutomaticReconnect()
      .build()

    connection.start().then(() => {
      connection.invoke("Connect")
    })

    setConnection(connection)
    //#endregion
  }, [url])

  useEffect(() => {
    if (connection == null) return

    //#region Socket messages
    //#region Game state
    connection.on("RecieveGameStarted", (data) => {
      const { pickedCards, turn } = data

      let startingCards = []

      pickedCards
        .sort(function (a, b) {
          return a - b
        })
        .forEach((value) => {
          startingCards.push(valueToCard(value, viewColor))
        })

      onRecieveGameMessage(
        `Game started, ${
          colors[turn]
        } starts. Playing cards: ${startingCards.toString()}`
      )
      setStartedResponse(data)
      setTurn(colors[turn])
    })

    connection.on("RecieveGameEnded", (data) => {
      const { way, result } = data

      onRecieveGameMessage(
        `Game ended, player ${colors[result]} won. Reason: ${reasons[way]}.`
      )
      setEndedResponse(data)
    })

    connection.on("RecieveConnected", (data) => {
      const { playingColor, state } = data

      setPlayingColor(colors[playingColor])
      setColorView(colors[playingColor])

      const {
        startedResponse,
        endedResponse,
        redPlayerState,
        blackPlayerState,
        boardState,
        timestamp,
        turn,
        turnCount,
      } = state

      if (startedResponse == null) {
        onRecieveGameMessage(
          `Playing as ${colors[playingColor]}. Waiting for rival to connect...`
        )
      }
      else if (endedResponse == null) {
        console.log("hey")
        onRecieveGameMessage(
          `Game already started. It's ${
            colors[turn]
          }'s turn. Playing cards: ${startedResponse.startingCards.toString()}`
        )
      } else {
        const { way, result } = endedResponse
        onRecieveGameMessage(
          `Game ended, ${colors[result]} won due to ${
            reasons[way]
          }. Playing cards: ${startedResponse.startingCards.toString()}`
        )
      }

      setStartedResponse(startedResponse)
      setEndedResponse(endedResponse)
      setRedPlayer(redPlayerState)
      setBlackPlayer(blackPlayerState)
      setBoardState(boardState)
      setTurn(colors[turn])
      setTurnCount(turnCount)
      setLastTurnTimeStamp(timestamp)
    })

    //#region Actions
    connection.on("RecieveMove", (response) => {
      const { data, timestamp } = response
      const { from, to } = data

      console.log(response)

      let newCardStackStates = boardState.cardStackStates
      delete newCardStackStates[from]
      newCardStackStates[to] = boardState.cardStackStates[from]

      console.log(newCardStackStates)

      setBoardState({
        ...boardState,
        cardStackStates: newCardStackStates,
      })

      nextTurn(timestamp)
    })

    connection.on("RecieveAttack", (data) => {
      console.log(data)
    })

    connection.on("RecievePassing", (data) => {
      console.log(data)
    })

    connection.on("RecieveSee", (data) => {
      console.log(data)
    })

    connection.on("RecieveOwnerSee", (data) => {
      console.log(data)
    })
    //#endregion

    //#region Decisions
    connection.on("RecieveReportIllegal", (data) => {
      console.log(data)
    })

    connection.on("RecieveTimeAdd", (data) => {
      console.log(data)
    })
    //#endregion

    //#region Messaging and errors
    connection.on("RecieveMessage", onRecieveUserMessage)

    connection.on("RecieveError", (response) => {
      console.error(response)
    })
    //#endregion
    //#endregion

    return () => {
      //#region Unsubscribe socket messages
      //#region Game state
      connection.off("RecieveGameStarted")
      connection.off("RecieveGameEnded")
      connection.off("RecieveConnected")
      //#endregion

      //#region Actions
      connection.off("RecieveMove")
      connection.off("RecieveAttack")
      connection.off("RecievePassing")
      connection.off("RecieveSee")
      connection.off("RecieveOwnerSee")
      //#endregion

      //#region Decisions
      connection.off("RecieveReportIllegal")
      connection.off("RecieveTimeAdded")
      //#endregion

      //#region Messaging and errors
      connection.off("RecieveMessage")
      connection.off("RecieveError")
      //#endregion
      //#endregion
    }
  }, [
    connection,
    boardState,
    turnCount,
    setBoardState,
    setTurnCount,
    nextTurn,
    onRecieveUserMessage,
    onRecieveGameMessage,
  ])
  //#endregion

  return (
    <gameContext.Provider
      value={{
        connection,
        viewColor,
        startedResponse,
        endedResponse,
        boardState,
        playingColor,
        turn,
        turnCount,
        players: {
          red: redPlayer,
          black: blackPlayer,
        },
        lastTurnTimeStamp,
        messages,
      }}
    >
      <div className="Game padded">
        <div className="Game-board-view" viewColor={viewColor}>
          <Profile playerColor="black" />
          <Board />
          <Profile playerColor="red" />
        </div>
        <Chat />
      </div>
    </gameContext.Provider>
  )
}

export default Game
