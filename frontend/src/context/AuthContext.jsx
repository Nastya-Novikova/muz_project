import React, { createContext, useState, useContext, useEffect } from 'react';

const generateAvatarUrl = (nameOrEmail) => {
  const name = nameOrEmail || 'User';
  const initials = name
    .split(' ')
    .map(part => part.charAt(0).toUpperCase())
    .join('')
    .substring(0, 2);

  const encodedName = encodeURIComponent(initials);
  return `https://ui-avatars.com/api/?name=${encodedName}&background=random&size=128`;
};

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(() => {
    const savedUser = localStorage.getItem('musicianFinder_user');
    const savedToken = localStorage.getItem('musicianFinder_token');
    if (savedUser && savedToken) {
      const parsedUser = JSON.parse(savedUser);
      if (!parsedUser.avatar) {
        parsedUser.avatar = generateAvatarUrl(parsedUser.fullName || parsedUser.email);
        localStorage.setItem('musicianFinder_user', JSON.stringify(parsedUser));
      }
      return { ...parsedUser, token: savedToken };
    }
    return null;
  });

  const [isNewUser, setIsNewUser] = useState(false);

  const login = (userData, token) => {
    const userToStore = { ...userData };
    if (!userToStore.avatar) {
      userToStore.avatar = generateAvatarUrl(userData.fullName || userData.email);
    }

    const userWithToken = { ...userToStore, token }; 
    setUser(userWithToken);
    setIsNewUser(!userToStore.profileCreated); 
    localStorage.setItem('musicianFinder_user', JSON.stringify(userToStore));
    localStorage.setItem('musicianFinder_token', token);
  };

  const logout = () => {
    setUser(null);
    setIsNewUser(false);
    localStorage.removeItem('musicianFinder_user');
    localStorage.removeItem('musicianFinder_token');
  };

  const updateProfile = (updatedData) => {
    if (!user) return;
    const needsNewAvatar = (!user.fullName && updatedData.fullName) || (user.fullName !== updatedData.fullName);
    const newAvatar = needsNewAvatar && !updatedData.avatar ? generateAvatarUrl(updatedData.fullName || user.email) : (updatedData.avatar || user.avatar);

    const updatedUser = {
      ...user,
      ...updatedData,
      avatar: newAvatar
    };
    setUser(updatedUser);
    setIsNewUser(false);
    localStorage.setItem('musicianFinder_user', JSON.stringify(updatedUser));
  };

  // Получения токена
  const getToken = () => {
    return user?.token || localStorage.getItem('musicianFinder_token');
  };

  // Проверки аутентификации
  const isAuthenticated = !!user;

  return (
    <AuthContext.Provider value={{
      user,
      login,
      logout,
      updateProfile,
      setIsNewUser,
      getToken,
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