import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAuth } from '../../../context/AuthContext';
import { api } from '../../../services/api';
import Header from '../../../components/Header/Header';
import ConfirmDeleteModal from '../../../components/ConfirmDeleteModal/ConfirmDeleteModal';
import SuggestionModal from '../../../components/SuggestionModal/SuggestionModal';
import ProfilePageView from './ProfilePageView';

function ProfilePage() {
  const { userId } = useParams();
  const { getToken, logout, getUserEmail, getUserRole } = useAuth(); 
  const navigate = useNavigate();
  
  const [activeTab, setActiveTab] = useState('info');
  const [profileData, setProfileData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [isFavorite, setIsFavorite] = useState(false);
  const [isCheckingFavorite, setIsCheckingFavorite] = useState(false);
  const [isCollaboration, setIsCollaboration] = useState(false);
  const [sendingCollaboration, setSendingCollaboration] = useState(false);
  const [currentUserId, setCurrentUserId] = useState(null);
  const [mediaData, setMediaData] = useState(null);
  const [avatarUrl, setAvatarUrl] = useState('/default-avatar.png');

  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);
  const [isSuggestionModalOpen, setIsSuggestionModalOpen] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);

  const checkFavoriteStatus = async (profileId, token) => {
    if (!profileId || isOwnProfile) return;
    
    setIsCheckingFavorite(true);
    try {
      const response = await api.checkIsFavorite(profileId, token);
      setIsFavorite(response.isFavorite || false);
    } catch (err) {
      console.error('Ошибка проверки избранного:', err);
    } finally {
      setIsCheckingFavorite(false);
    }
  };

  const checkCollaborationStatus = async (profileId, token) => {
    if (!profileId || isOwnProfile) return;
    
    setSendingCollaboration(true);
    try {
      const response = await api.checkCollaboration(profileId, token);
      setIsCollaboration(response.isCollaborated || false);
    } catch (err) {
      console.error('Ошибка проверки предложения:', err);
    } finally {
      setSendingCollaboration(false);
    }
  };

  useEffect(() => {
    const loadProfile = async () => {
      setLoading(true);
      setError('');
      
      try {
        const token = getToken();
        const myProfile = await api.getProfile(token);
        setCurrentUserId(myProfile.id);
        
        if (!userId) {
          // Свой профиль
          setProfileData(myProfile);

          if (myProfile.avatar) {
            setAvatarUrl(api.getAvatarUrl(myProfile.avatarUrl));
          }

          const media = await api.getMedia(myProfile.id, token);
          setMediaData(media);
        } else {
          // Чужой профиль
          const otherProfileData = await api.getProfileById(userId);
          if (otherProfileData.avatar) { 
            setAvatarUrl(api.getAvatarUrl(otherProfileData.avatarUrl));
          }

          setProfileData(otherProfileData);

          const media = await api.getMedia(userId, token);
          setMediaData(media);

          const isViewingOwnProfile = userId === myProfile.id;
          if (!isViewingOwnProfile) {
            await checkFavoriteStatus(userId, token);
            await checkCollaborationStatus(userId, token);
          }
        }
      } catch (err) {
        console.error('Ошибка загрузки профиля:', err);
        if (err.message.includes('401') || err.message.includes('Unauthorized')) {
          logout();
          navigate('/login');
          return;
        }
        setError('Не удалось загрузить профиль. Пожалуйста, попробуйте позже.');
      } finally {
        setLoading(false);
      }
    };

    loadProfile();
  }, [userId, getToken, navigate, logout]);

  useEffect(() => {
    if (profileData?.avatarUrl) {
      const url = api.getAvatarUrl(profileData.avatarUrl);
      setAvatarUrl(url);
    }
  }, [profileData]);

  const isOwnProfile = !userId || (userId && currentUserId && userId === currentUserId);
  const userEmail = getUserEmail();
  const userRole = getUserRole();

  const handleEditProfile = () => {
    navigate('/profile/edit');
  };

  const handleOpenDeleteModal = () => {
    setIsDeleteModalOpen(true);
  };

  const handleConfirmDelete = async () => {
    setIsDeleting(true);
    try {
      const token = getToken();
      await api.deleteProfile(token);
      logout(); 
      navigate('/');
    } catch (err) {
      console.error('Ошибка при удалении профиля:', err);
      alert('Не удалось удалить профиль. Пожалуйста, попробуйте позже.');
    } finally {
      setIsDeleting(false);
      setIsDeleteModalOpen(false);
    }
  };

  const handleBack = () => {
    navigate(-1);
  };

  const handleToggleFavorite = async () => {
    if (isOwnProfile || !userId) return;
    
    try {
      const token = getToken();
      
      if (isFavorite) {
        await api.removeFromFavorites(userId, token);
        setIsFavorite(false);
      } else {
        await api.addToFavorites(userId, token);
        setIsFavorite(true);
      }
    } catch (err) {
      console.error('Ошибка обновления избранного:', err);
      alert('Не удалось обновить избранное');
    }
  };

  const handleCollaboration = async (message) => {
    if (isOwnProfile || !userId || isCollaboration) return;
    
    try {
      const token = getToken();
      
      if (!isCollaboration) {
        await api.sendSuggestion(userId, message, token);
        setIsCollaboration(true);
      }
    } catch (err) {
      console.error('Ошибка отправки предложения:', err);
      alert('Не удалось отправить предложение');
    } 
  };

  const handleOpenSuggestionModal = () => {
    setIsSuggestionModalOpen(true);
  };

  const getLookingForText = () => {
    if (!profileData) return '';
    switch(profileData.lookingFor) {
      case 'LookingForBand': return 'Ищет коллектив';
      case 'LookingForMusician': return 'Ищет музыкантов';
      default: return '';
    }
  };

  const getRoleText = () => {
    if (!profileData) return '';
    return profileData.profileType === 'Band' ? 'Коллектив' : 'Музыкант';
  };

  if (loading) {
    return (
      <>
        <Header />
        <div className="profile-page">
          <div className="profile-container">
            <div className="loading-spinner">Загрузка...</div>
          </div>
        </div>
      </>
    );
  }

  if (error) {
    return (
      <>
        <Header />
        <div className="profile-page">
          <div className="profile-container">
            <p>Ошибка: {error}</p>
            <button onClick={handleBack} className="back-btn">Назад</button>
          </div>
        </div>
      </>
    );
  }

  if (!profileData) {
    return (
      <>
        <Header />
        <div className="profile-page">
          <div className="profile-container">
            <p>Пользователь не найден</p>
            <button onClick={handleBack} className="back-btn">Назад</button>
          </div>
        </div>
      </>
    );
  }

  return (
    <>
      <ProfilePageView
        profileData={profileData}
        avatarUrl={avatarUrl}
        activeTab={activeTab}
        setActiveTab={setActiveTab}
        isOwnProfile={isOwnProfile}
        userEmail={userEmail}
        userRole={userRole}
        isFavorite={isFavorite}
        isCheckingFavorite={isCheckingFavorite}
        isCollaboration={isCollaboration}
        sendingCollaboration={sendingCollaboration}
        mediaData={mediaData}
        onEditProfile={handleEditProfile}
        onOpenDeleteModal={handleOpenDeleteModal}
        onBack={handleBack}
        onToggleFavorite={handleToggleFavorite}
        onOpenSuggestionModal={handleOpenSuggestionModal}
        getLookingForText={getLookingForText}
        getRoleText={getRoleText}
      />
      
      <ConfirmDeleteModal
        isOpen={isDeleteModalOpen}
        onClose={() => setIsDeleteModalOpen(false)}
        onConfirm={handleConfirmDelete}
        userName={profileData?.fullName || 'Профиль'}
      />
      
      <SuggestionModal
        isOpen={isSuggestionModalOpen}
        onClose={() => setIsSuggestionModalOpen(false)}
        onSend={handleCollaboration}
        userName={profileData?.fullName || 'пользователю'}
      />
    </>
  );
}

export default ProfilePage;