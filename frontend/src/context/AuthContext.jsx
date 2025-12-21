import React, { createContext, useState, useContext, useEffect } from 'react';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(() => {
    const savedUser = localStorage.getItem('musicianFinder_user');
    return savedUser ? JSON.parse(savedUser) : null;
  });

  const [isNewUser, setIsNewUser] = useState(false);

  const login = (userData) => {
    setUser(userData);
    localStorage.setItem('musicianFinder_user', JSON.stringify(userData));
    setIsNewUser(!userData.profileCompleted);
  };

  const logout = () => {
    setUser(null);
    setIsNewUser(false);
    localStorage.removeItem('musicianFinder_user');
  };

  const updateProfile = (updatedData) => {
    const updatedUser = {
      ...user,
      ...updatedData,
      profileCompleted: true
    };
    setUser(updatedUser);
    setIsNewUser(false);
    localStorage.setItem('musicianFinder_user', JSON.stringify(updatedUser));
  };

  return (
    <AuthContext.Provider value={{ 
      user, 
      login, 
      logout, 
      updateProfile,
      isNewUser,
      setIsNewUser 
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