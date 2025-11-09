import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'
import LoginOTP from './components/login'

function App() {
  const [count, setCount] = useState(0)
  const handleLogin = (email) => {
    alert('Вы вошли как: ${email}');
  };

  return (
    <div className="App">
      <LoginOTP onLogin={handleLogin} />
    </div>
  );
}

export default App
