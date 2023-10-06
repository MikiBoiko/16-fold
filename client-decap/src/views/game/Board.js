import { useCallback, useContext, useEffect, useState } from "react"
import Tile from "./Tile"
import gameContext from "../../context/gameContext"
import boardContext from "../../context/boardContext"
import "./Board.css"

const Board = () => {
  const { connection, viewColor: colorView, playingColor, boardState } = useContext(gameContext)
  const [state, setState] = useState({})
  const [action, setAction] = useState({
    type: null,
    data: {},
  })

  useEffect(
    (colorView) => {
      if (boardState == null) return

      setState(boardState.cardStackStates)
    },
    [boardState, colorView, setState]
  )

  const onCancel = useCallback(() => {
    setAction({
      type: null,
      data: {},
    })
  }, [setAction])

  const onSelect = useCallback(
    (from) => {
      setAction({
        type: "Select",
        data: {
          from,
        },
      })
    },
    [setAction]
  )

  const onAttack = useCallback(
    (to) => {
      setAction({
        type: "Attack",
        data: {
          from: [action.data.from],
          to,
        },
      })
    },
    [action, setAction]
  )

  const onAddToAttack = useCallback(
    (from) => {
      setAction({
        ...action,
        data: {
          ...action.data,
          from: [...action.data.from, from],
        },
      })
    },
    [action, setAction]
  )

  const onDoAction = useCallback(
    (actionRequest) => {
      console.log(actionRequest)
      connection.invoke("DoAction", actionRequest)
      onCancel()
    },
    [connection, onCancel]
  )

  return (
    <boardContext.Provider
      value={{
        action,
        onCancel,
        onSelect,
        onAttack,
        onAddToAttack,
        onDoAction,
      }}
    >
      <div className="Board-container">
        <div className="Board" colorview={colorView}>
          {["1", "2", "3", "4", "5", "6", "7"].map((row, rowIndex) => {
            return (
              <div key={rowIndex} className="Board-row" colorview={colorView}>
                {["a", "b", "c", "d"].map((column, columnIndex) => {
                  const tile = column + row
                  const card = state != null ? state[tile] : null
                  /*console.log(
                    tile,
                    columnIndex,
                    rowIndex,
                    rowIndex * 4 + columnIndex
                  )*/
                  return (
                    <Tile
                      key={columnIndex}
                      index={rowIndex * 7 + columnIndex}
                      position={tile}
                      hasCard={tile in state}
                      playingColor={playingColor}
                      card={card}
                    />
                  )
                })}
              </div>
            )
          })}
        </div>
      </div>
    </boardContext.Provider>
  )
}

export default Board
