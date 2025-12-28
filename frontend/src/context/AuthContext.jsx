import React, { createContext, useState, useContext } from 'react';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [token, setToken] = useState(() => {
    return localStorage.getItem('musicianFinder_token');
  });

  const login = (userData, authToken) => {
    setToken(authToken);
    localStorage.setItem('musicianFinder_token', authToken);
    // Сохраняем только email для отображения в интерфейсе
    localStorage.setItem('user_email', userData.email);
  };

  const logout = () => {
    setToken(null);
    localStorage.removeItem('musicianFinder_token');
    localStorage.removeItem('user_email');
  };

  const getToken = () => {
    return token || localStorage.getItem('musicianFinder_token');
  };

  const getUserEmail = () => {
    return localStorage.getItem('user_email');
  };

  const isAuthenticated = !!getToken();

  return (
    <AuthContext.Provider value={{
      token,
      login,
      logout,
      getToken,
      getUserEmail,
      isAuthenticated
    }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};