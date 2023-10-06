import { IonButton } from "@ionic/react"
import { createContext, useCallback, useContext, useEffect, useState } from "react"


interface ContextA {
  timestamp: number | undefined,
  turn: number | undefined
}

const context = createContext<ContextA>({
  timestamp: undefined,
  turn: undefined
})

interface TimerProps {
  color: number,
  interval: number
}

const parsePlayerTime = (time: number) => {
  const timeClamped = time > 0 ? time : 0

  const hours = Math.floor(timeClamped / 3600000)
    .toString()
    .padStart(2, "0")
  const minutes = Math.floor((timeClamped / 60000) % 60)
    .toString()
    .padStart(2, "0")
  const seconds = Math.floor((timeClamped / 1000) % 60)
    .toString()
    .padStart(2, "0")
  const miliseconds = Math.floor(timeClamped % 1000)
    .toString()
    .padStart(3, "0")

  return `${hours === "00" ? "" : `${hours}:`
    }${minutes}:${seconds}.${miliseconds}`
}

const Timer = ({ color, interval }: TimerProps) => {
  const { timestamp, turn } = useContext<ContextA>(context)

  const [time, setTime] = useState<number>(interval)

  const isTurn = color === turn
  const timestampEnd = (timestamp ?? Date.now()) + time

  useEffect(() => {
    const interval = setInterval(() => {
      if (isTurn) {
        setTime(timestampEnd - Date.now())
      }
    }, 100)

    return () => clearInterval(interval)
  }, [isTurn, timestamp])

  return (
    <div>
      {parsePlayerTime(time)}
      {isTurn ? '(*)' : ''}
    </div>
  )
}

const TimerTest = () => {
  const [timestamp, setTimestamp] = useState<number | undefined>()
  const [turn, setTurn] = useState<number | undefined>()

  const start = useCallback(() => {
    setTimestamp(Date.now()),
      setTurn(Math.floor(Math.floor(Math.random() * 2)))

    console.log({
      timestamp: Date.now(),
      turn: Math.floor(Math.floor(Math.random() * 2))
    })
  }, [setTimestamp, setTurn])

  const nextTurn = useCallback(() => {
    if (turn === undefined) {
      console.error('you are stupid')
      return
    }

    setTimestamp(Date.now())
    const nextTurnIndex = (turn + 1) % 2
    setTurn(nextTurnIndex)
  }, [setTimestamp, setTurn, turn])

  return (
    <context.Provider
      value={{
        timestamp,
        turn
      }}
    >
      <div id="container" className="ion-flex">
        <IonButton onClick={start}>Start</IonButton>
        <Timer color={0} interval={70000} />
        <Timer color={1} interval={50000} />
        <IonButton onClick={nextTurn}>Next</IonButton>
      </div>
    </context.Provider>
  )
}

export default TimerTest