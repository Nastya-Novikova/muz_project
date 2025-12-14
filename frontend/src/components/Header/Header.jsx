import React from 'react';
import { Link } from 'react-router-dom';
import './Header.css';

function Header() {
  return (
    <header className="header">
      <div className="header-content">
        <div className="header-left">
          <Link to="/" className="logo-link">
            <h1 className="logo">MusicianFinder</h1>
          </Link>
        </div>
        <div className="header-right">
          <Link to="/login" className="login-link">Войти</Link>
        </div>
      </div>
    </header>
  );
}

export default Header;