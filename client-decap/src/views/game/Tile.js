import { useCallback, useContext, useEffect, useState } from "react"
import Card from "./Card"
import boardContext from "../../context/boardContext"
import { adjacentTo } from "./misc"
import "./Tile.css"

const Tile = (props) => {
  const { position, index, playingColor } = props
  const card = props.card ?? { value: null, color: null }
  const hasCard = props.hasCard

  const { action, onCancel, onSelect, onAttack, onAddToAttack, onDoAction } =
    useContext(boardContext)

  const [move, setMove] = useState("")

  const onClick = useCallback(() => {
    switch (move) {
      case "None":
        break
      case "Move":
        onDoAction({
          type: "Move",
          data: {
            from: action.data.from,
            to: position,
          },
        })
        break
      case "See":
        onDoAction({
          type: "See",
          data: { from: position },
        })
        break
      case "Cancel":
        onCancel()
        break
      case "Target":
        onAttack(position)
        break
      case "Add":
        onAddToAttack(position)
        break
      case "Select":
        onSelect(position)
        break
      case "Confirm":
        onDoAction({
          type: "Attack",
          data: {
            from: action.data.from,
            to: action.data.to,
          },
        })
        break
      case "Passing":
        onDoAction({
          type: "Passing",
          data: {
            from: action.data.from,
          },
        })
        break
      default:
        break
    }
  }, [
    position,
    action,
    move,
    onCancel,
    onSelect,
    onAttack,
    onAddToAttack,
    onDoAction,
  ])

  useEffect(() => {
    if (playingColor === "none" || playingColor === "both") return

    if (action.type == null) {
      if (hasCard) {
        setMove("Select")
      } else setMove("None")
      return
    }
    switch (action.type) {
      case "Select":
        if (position === action.data.from) {
          if (position[1] === (playingColor === "red" ? "7" : "1")) {
            setMove("Passing")
          } else setMove("See")
        } else if (adjacentTo(position, action.data.from)) {
          setMove(hasCard ? "Target" : "Move")
        } else setMove("Cancel")
        break
      case "Attack":
        if (position === action.data.to) setMove("Confirm")
        else if (
          hasCard &&
          adjacentTo(position, action.data.to) &&
          action.data.from.includes(position) === false
        )
          setMove("Add")
        else setMove("Cancel")
        break
      default:
        console.error("Wrong action type.")
        break
    }
  }, [position, action, playingColor, hasCard, setMove])

  return (
    <button
      className="Tile"
      onClick={() => onClick()}
      handness={index % 2 === 0 ? "even" : "odd"}
    >
      {hasCard ? (
        <Card value={card["value"]} color={card["color"]} />
      ) : (
        <div></div>
      )}
      <div className="Tile-Icon" move={move} />
    </button>
  )
}

export default Tile
