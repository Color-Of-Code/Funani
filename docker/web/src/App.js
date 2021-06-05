import logo from "./logo.svg";

import Login from "./Login";

import "./App.css";

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <p>FUNANI</p>
        <Login></Login>
      </header>
    </div>
  );
}

export default App;
