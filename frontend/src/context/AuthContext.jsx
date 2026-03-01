import React, { createContext, useState, useContext } from 'react';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [token, setToken] = useState(() => {
    return localStorage.getItem('musicianFinder_token');
  });

  const [userRole, setUserRole] = useState(() => {
    return localStorage.getItem('userRole') || null;
  });

  const login = (userData, authToken) => {
    setToken(authToken);
    localStorage.setItem('musicianFinder_token', authToken);
    localStorage.setItem('user_email', userData.email);
    if (userData.role) {
      localStorage.setItem('userRole', userData.role);
      setUserRole(userData.role);
    }
  };

  const setRole = (role) => {
    localStorage.setItem('userRole', role);
    setUserRole(role);
  };

  const logout = () => {
    setToken(null);
    setUserRole(null);
    localStorage.removeItem('musicianFinder_token');
    localStorage.removeItem('user_email');
    localStorage.removeItem('userRole');
  };

  const getToken = () => {
    return token || localStorage.getItem('musicianFinder_token');
  };

  const getUserEmail = () => {
    return localStorage.getItem('user_email');
  };

  const getUserRole = () => {
    return userRole || localStorage.getItem('userRole');
  };

  const isAuthenticated = !!getToken();

  return (
    <AuthContext.Provider value={{
      token,
      login,
      logout,
      setRole,
      getToken,
      getUserEmail,
      getUserRole,
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