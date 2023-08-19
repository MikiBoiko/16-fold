import { createContext } from "react"

const boardContext = createContext({
  action: {
    type: null,
    data: {},
  },
  onCancel: () => {},
  onSelect: (from) => {},
  onAttack: (to) => {},
  onAddToAttack: (from) => {},
  onDoAction: (actionRequest) => {},
})

export default boardContext
