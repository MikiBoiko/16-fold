import { useState, useEffect, useContext, useCallback } from "react"
import gameContext from "../../context/gameContext"
import profile from "../../imgs/profile.png"
import addTimeIcon from "../../imgs/decision/add_time.png"
import reportIcon from "../../imgs/decision/report.png"
import { colors } from "./misc"
import "./Profile.css"

const parsePlayerTime = (time) => {
  const hours = Math.floor(time / 3600000)
    .toString()
    .padStart(2, "0")
  const minutes = Math.floor((time / 60000) % 60)
    .toString()
    .padStart(2, "0")
  const seconds = Math.floor((time / 1000) % 60)
    .toString()
    .padStart(2, "0")
  const miliseconds = Math.floor(time % 1000)
    .toString()
    .padStart(3, "0")

  return `${
    hours === "00" ? "" : `${hours}:`
  }${minutes}:${seconds}.${miliseconds}`
}

const Profile = (props) => {
  const playerColor = props.playerColor

  const {
    connection,
    players,
    lastTurnTimeStamp,
    playingColor,
    turn,
    turnCount,
    endedResolution,
  } = useContext(gameContext)

  const onReportIllegal = useCallback(() => {
    console.log({ type: "ReportIllegal" })
    connection.invoke("DoDecision", { type: "ReportIllegal" })
  }, [connection])

  const onAddTime = useCallback(() => {
    console.log({ type: "AddTime" })
    connection.invoke("DoDecision", { type: "AddTime" })
  }, [connection])

  const player = players[playerColor]
  const isPlayerNull = player == null
  const isPlaying = playerColor === playingColor

  const [timeLeft, setTimeLeft] = useState()
  const [timeStatus, setTimeStatus] = useState("wait")

  useEffect(() => {
    setTimeStatus(turn !== playerColor ? "wait" : "turn")
  }, [turn, playerColor, setTimeStatus])

  useEffect(() => {
    if (isPlayerNull) return

    setTimeLeft(player.playerTimerState.interval)
    const interval = setInterval(() => {
      setTimeLeft(
        lastTurnTimeStamp + player.playerTimerState.interval - Date.now()
      )
    }, 500)

    return () => clearInterval(interval)
  }, [
    timeLeft,
    turnCount,
    lastTurnTimeStamp,
    player,
    isPlayerNull,
    setTimeLeft,
  ])

  return (
    <div className="Profile" playercolor={playerColor}>
      <div className="Profile-tag">
        <img className="Profile-icon" src={profile} alt="" />
        <div className="Profile-user">
          <div className="Profile-username">
            {isPlayerNull ? null : player.username}
            {colors[(endedResolution ?? { result: null }).result ?? 3] ===
            playerColor
              ? " (WINNER)"
              : null}
          </div>
          <div className="Profile-elo">
            {isPlayerNull ? "1500?" : player.elo ?? ""}
          </div>
        </div>
      </div>
      <div className="Profile-Time-Decisions">
        {playerColor === "none" ? null : (
          <button
            className="Profile-Decision"
            onClick={() => {
              if (isPlaying) {
                onReportIllegal()
              } else {
                onAddTime()
              }
            }}
          >
            <img
              className="Profile-decision-icon"
              src={isPlaying === true ? reportIcon : addTimeIcon}
              alt=""
            />
          </button>
        )}
        <div className="Profile-time" status={timeStatus}>
          {parsePlayerTime(
            isPlayerNull || timeLeft <= 0 || isNaN(timeLeft) ? 0 : timeLeft
          )}
        </div>
      </div>
    </div>
  )
}

export default Profile
