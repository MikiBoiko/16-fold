import { valueToCard } from "./misc"
import "./Card.css"
import { useState } from "react"

const Card = (props) => {
  const value = props.value ?? null
  const color = props.color ?? null

  const [valued, setValued] = useState(value)

  return (
    <div className="Card">
      <div
        className={`Card-image${valued === true ? " valued" : ""}`}
        color={color}
      >
        {valued != null ? valueToCard(value, color) : null}
      </div>
    </div>
  )
}

export default Card
