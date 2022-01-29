// see https://github.com/facebook/create-react-app/issues/11770
// import logo from "./logo.svg";
// <img src={logo} className="App-logo" alt="logo" />

import Login from "./Login";

import "./App.css";

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <p>FUNANI</p>
        <Login></Login>
      </header>
    </div>
  );
}

export default App;
