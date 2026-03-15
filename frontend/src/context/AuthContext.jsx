import React, { createContext, useState, useContext, useEffect } from 'react';
import { api } from '../services/api';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [token, setToken] = useState(() => {
    return localStorage.getItem('musicianFinder_token');
  });

  const [profile, setProfile] = useState(null);

  useEffect(() => {
    if (token) {
      loadProfile();
    }
  }, [token]);

  const loadProfile = async () => {
    try {
      const profileData = await api.getProfile(token);
      setProfile(profileData);
    } catch (error) {
      console.log('Профиль не найден');
      setProfile(null);
    }
  };

  const login = (userData, authToken) => {
    setToken(authToken);
    localStorage.setItem('musicianFinder_token', authToken);
    localStorage.setItem('user_email', userData.email);
  };

  const setRole = (role) => {
    localStorage.setItem('userRole', role);
  };

  const logout = () => {
    setToken(null);
    setProfile(null);
    localStorage.removeItem('musicianFinder_token');
    localStorage.removeItem('user_email');
    localStorage.removeItem('userRole');
  };

  const getToken = () => token;

  const getUserEmail = () => {
    return localStorage.getItem('user_email');
  };

  const getUserRole = () => {
    if (profile?.profileType) {
      return profile.profileType;
    }
    return localStorage.getItem('userRole');
  };

  const isAuthenticated = !!token;

  return (
    <AuthContext.Provider value={{
      login,
      logout,
      setRole,
      getToken,
      getUserEmail,
      getUserRole,
      isAuthenticated,
      profile
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