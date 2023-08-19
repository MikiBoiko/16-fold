import { useCallback, useState, useContext } from "react"
import gameContext from "../../context/gameContext"
import "./Chat.css"

const UserMessage = (props) => {
  const { username, content } = props.message

  return (
    <div className="Chat-user-message">
      <strong>{username}</strong>:{" " + content}
    </div>
  )
}

const GameMessage = (props) => {
  const { message } = props

  return <div className="Chat-game-message">{message}</div>
}

const Chat = (props) => {
  const [userMessage, setUserMessage] = useState("")

  const { connection, playingColor, messages } = useContext(gameContext)

  const onSendUserMesage = useCallback(
    (message) => {
      if(message.length < 1) return

      connection.invoke("SendMessage", message)

      setUserMessage("")
    },
    [connection, setUserMessage]
  )

  return (
    <div className="Chat">
      <div className="Chat-messages">
        {messages.map((message, index) => {
          return message.type === "user" ? (
            <UserMessage key={index} message={message.data} />
          ) : (
            <GameMessage key={index} message={message.data} />
          )
        })}
      </div>
      <div className="Chat-user">
        <textarea
          placeholder="Write a message..."
          className="Chat-textarea"
          onChange={(e) => setUserMessage(e.target.value)}
          value={userMessage}
        />
        <button
          className="Chat-button"
          disabled={playingColor === "none"}
          onClick={() => onSendUserMesage(userMessage)}
        >
          SEND
        </button>
      </div>
    </div>
  )
}

export default Chat
