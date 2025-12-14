import { useState } from 'react'
import './App.css'
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import HomePage from './pages/Home/HomePage';
import LoginOTP from './pages/Login/LoginPage';

function App() {
  const handleLogin = (email) => {
    alert(`Вы вошли как: ${email}`);
  };

  return (
    <Router>
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/login" element={<LoginOTP onLogin={handleLogin} />} />
      </Routes>
    </Router>
  );
}

export default App;
