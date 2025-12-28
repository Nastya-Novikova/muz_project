import React, { useState, useRef, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import './Header.css';

function Header() {
  const [dropdownOpen, setDropdownOpen] = useState(false);
  const dropdownRef = useRef(null);
  const { isAuthenticated, logout, getUserEmail } = useAuth(); // Меняем user на isAuthenticated
  const navigate = useNavigate();

  useEffect(() => {
    const handleClickOutside = (event) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
        setDropdownOpen(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, []);

  const toggleDropdown = () => {
    setDropdownOpen(!dropdownOpen);
  };

  const handleProfileClick = () => {
    setDropdownOpen(false);
    navigate('/profile');
  };

  const handleSearchClick = () => {
    setDropdownOpen(false);
    navigate('/');
  };

  const handleSuggestionsClick = () => {
    setDropdownOpen(false);
    navigate('/suggestions');
  };

  const handleLogout = () => {
    setDropdownOpen(false);
    logout();
    navigate('/login');
  };

  // Получаем email пользователя
  const userEmail = getUserEmail();
  const userEmailLogo = userEmail.split('@')[0];

  return (
    <header className="header">
      <div className="header-content">
        <div className="header-left">
          <Link to="/" className="logo-link">
            <h1 className="logo">MusicianFinder</h1>
          </Link>
        </div>
        <div className="header-right">
          {isAuthenticated ? ( // Проверяем isAuthenticated вместо user
            <div className="user-menu" ref={dropdownRef}>
              <button 
                className="user-email-logo" 
                onClick={toggleDropdown}
                aria-label="Меню пользователя"
              >
                <span className="user-email-logo">{userEmailLogo || ''}</span>
              </button>
              
              {dropdownOpen && (
                <div className="dropdown-menu">
                  <div className="user-info">
                    <span className="user-email">{userEmail || ''}</span>
                  </div>
                  
                  <div className="dropdown-divider"></div>
                  
                  <button 
                    onClick={handleSearchClick} 
                    className="dropdown-item"
                  >
                    Поиск
                  </button>
                  
                  <button 
                    onClick={handleProfileClick} 
                    className="dropdown-item"
                  >
                    Профиль
                  </button>
                  
                  <button 
                    onClick={handleSuggestionsClick} 
                    className="dropdown-item"
                  >
                    Предложения
                  </button>
                  
                  <div className="dropdown-divider"></div>
                  
                  <button 
                    onClick={handleLogout} 
                    className="dropdown-item logout"
                  >
                    Выйти
                  </button>
                </div>
              )}
            </div>
          ) : (
            <Link to="/login" className="login-link">
              Войти
            </Link>
          )}
        </div>
      </div>
    </header>
  );
}

export default Header;