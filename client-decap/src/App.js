import Navbar from './components/Navbar'
import Game from './views/Game'
import './App.css'

function App() {
  return (
    <div className="App" theme="Dark">
      <Navbar />
      <div className="App-content">
        <Game url="http://localhost:5243" />
      </div>
    </div>
  )
}

export default App
