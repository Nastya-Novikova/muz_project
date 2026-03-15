import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { api } from '../../services/api';
import Header from '../../components/Header/Header';
import ConfirmDeleteModal from '../../components/ConfirmDeleteModal/ConfirmDeleteModal';
import SuggestionModal from '../../components/SuggestionModal/SuggestionModal';
import './ProfilePage.css';

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
      <Header />
      <div className="profile-page">
        <div className="profile-container">
          {/* Шапка профиля */}
          <div className="profile-header">
            <div className="profile-main-info">
              <img 
                src={avatarUrl}
                alt={profileData.fullName}
                className="profile-avatar" 
              />
              <div className="profile-info">
                <div className="profile-title-row">
                  <h1>{profileData.fullName || 'Не указано'}</h1>
                  <span className="profile-role-badge">{getRoleText()}</span>
                </div>
                {isOwnProfile && userEmail && (
                  <p className="profile-email">{userEmail}</p>
                )}
                <p className="profile-activity">
                  {profileData.age && `${profileData.age} ${profileData.profileType === 'Band' ? 'г.' : 'лет'}`}
                  {profileData.experience && ` • Стаж: ${profileData.experience} лет`}
                  {profileData.city.localizedName && ` • ${profileData.city.localizedName}`}
                </p>
                {profileData.lookingFor !== 'NotLooking' && (
                  <div className="profile-looking-badge">
                    {getLookingForText()}
                  </div>
                )}
              </div>
            </div>

            <div className="profile-actions-container">
              {isOwnProfile ? (
                <div className="profile-actions"> 
                  <button onClick={handleEditProfile} className="edit-profile-btn" title="Редактировать">
                    <img src='/pencil.png' alt='Редактировать' className="edit-profile-btn-img"/>
                  </button>
                  <button onClick={handleOpenDeleteModal} className="delete-profile-btn" title="Удалить">
                    <img src='/delete.png' alt='Удалить' className="delete-profile-btn-img"/>
                  </button>
                </div>
              ) : (
                <div className="profile-actions-btn">
                  <button
                    onClick={handleToggleFavorite}
                    className={`favorite-profile-btn ${isFavorite ? 'active' : ''}`}
                    disabled={isCheckingFavorite}
                  >
                    {isCheckingFavorite 
                      ? 'Проверка...' 
                      : isFavorite 
                        ? 'В избранном' 
                        : 'В избранное'}
                  </button>
                  <button onClick={handleBack} className="back-btn">Назад</button>
                  <button
                    onClick={handleOpenSuggestionModal}
                    className={`collaboration-btn ${isCollaboration ? 'sent' : ''}`}
                    disabled={isCollaboration}
                  >
                    {sendingCollaboration 
                      ? 'Отправка...' 
                      : isCollaboration 
                        ? 'Предложение направлено' 
                        : 'Предложить сотрудничество'}
                  </button>
                </div>
              )}
            </div>
          </div>

          {/* Табы */}
          <div className="profile-tabs">
            <button
              className={`tab-btn ${activeTab === 'info' ? 'active' : ''}`}
              onClick={() => setActiveTab('info')}
            >
              Основная информация
            </button>
            <button
              className={`tab-btn ${activeTab === 'looking' ? 'active' : ''}`}
              onClick={() => setActiveTab('looking')}
            >
              Поиск
            </button>
            <button
              className={`tab-btn ${activeTab === 'portfolio' ? 'active' : ''}`}
              onClick={() => setActiveTab('portfolio')}
            >
              Портфолио
            </button>
            <button
              className={`tab-btn ${activeTab === 'contacts' ? 'active' : ''}`}
              onClick={() => setActiveTab('contacts')}
            >
              Контакты
            </button>
          </div>

          {/* Контент табов */}
          <div className="profile-content">
            {activeTab === 'info' && (
              <div className="tab-content">
                <div className="info-section">
                  <h3>О себе</h3>
                  <p className="profile-description">
                    {profileData.description || 'Пользователь не добавил информацию о себе'}
                  </p>
                </div>

                <div className="info-grid">
                  <div className="info-item">
                    <span className="info-label">{profileData.profileType === 'Band' ? 'Год основания:' : 'Возраст:'}</span>
                    <span className="info-value">{profileData.age || 'Не указан'}</span>
                  </div>
                  <div className="info-item">
                    <span className="info-label">Город:</span>
                    <span className="info-value">{profileData.city.localizedName || 'Не указан'}</span>
                  </div>
                  <div className="info-item">
                    <span className="info-label">Стаж:</span>
                    <span className="info-value">
                      {profileData.experience ? `${profileData.experience} лет` : 'Не указан'}
                    </span>
                  </div>
                  <div className="info-item">
                      <span className="info-label">{profileData.profileType === 'Band' ? 'Состав коллектива:' : 'Вид деятельности:'}</span>
                      <span className="info-value">
                        {profileData.specialties?.map(s => s.localizedName).join(', ') || 'Не указан'}
                      </span>
                  </div>
                  <div className="info-item">
                      <span className="info-label">Жанры:</span>
                      <span className="info-value">
                        {profileData.genres?.map(g => g.localizedName).join(', ') || 'Не указаны'}
                      </span>
                  </div>
                </div>
              </div>
            )}

            {activeTab === 'looking' && (
              <div className="tab-content">
                <div className="info-section">
                  <h3>Поиск</h3>
                  
                  {profileData.lookingFor === 'NotLooking' ? (
                    <p className="profile-description">
                      {profileData.profileType === 'Band' 
                        ? 'Коллектив не ищет музыкантов' 
                        : 'Музыкант не ищет коллектив'}
                    </p>
                  ) : (
                    <>
                      {profileData.desiredGenres?.length > 0 && (
                        <>
                          <p className="looking-for-title">
                            {profileData.lookingFor === 'LookingForBand' 
                              ? 'Ищет коллектив с жанрами:' 
                              : 'Ищет музыкантов с жанрами:'}
                          </p>
                          <div className="looking-genres">
                            {profileData.desiredGenres.map(genre => (
                              <span key={genre.id} className="looking-tag">
                                {genre.localizedName}
                              </span>
                            ))}
                          </div>
                        </>
                      )}

                      {(!profileData.desiredGenres?.length) && (
                        <p className="looking-for-title">
                          {profileData.lookingFor === 'LookingForBand' 
                            ? 'Ищет коллектив' 
                            : 'Ищет музыкантов'}
                        </p>
                      )}

                      {profileData.profileType === 'Band' && profileData.desiredSpecialties?.length > 0 && (
                        <>
                          <p className="looking-for-title">Направления деятельности:</p>
                          <div className="looking-specialties">
                            {profileData.desiredSpecialties.map(spec => (
                              <span key={spec.id} className="looking-tag">
                                {spec.localizedName}
                              </span>
                            ))}
                          </div>
                        </>
                      )}
                    </>
                  )}
                </div>
              </div>
            )}

            {activeTab === 'portfolio' && (
              <div className="tab-content">
                {/* Секция фото */}
                <div className="portfolio-section">
                  <h3>Фотографии</h3>
                  {mediaData?.photos?.length > 0 ? (
                    <div className="photos-grid">
                      {mediaData.photos.map((photo) => (
                      <div key={photo.id} className="photo-item">
                        <img 
                          src={photo.fileUrl} 
                          alt={photo.title || 'Фото'} 
                          className="portfolio-photo" 
                        />
                        {photo.title && <p className="photo-title">{photo.title}</p>}
                      </div>
                     ))}
                    </div>
                  ) : (
                    <p className="no-content">Фотографии не загружены</p>
                  )}
                </div>

                {/* Секция аудио */}
                <div className="portfolio-section">
                  <h3>Аудиозаписи</h3>
                  {mediaData?.audio?.length > 0 ? (
                    <div className="audio-list">
                      {mediaData.audio.map((audio, index) => (
                      <div key={audio.id} className="audio-item">
                        <p className='audio-title'>{audio.title || `Аудиозапись ${index + 1}`}</p>
                        <audio className='audio-element' controls src={audio.fileUrl}>
                          Ваш браузер не поддерживает аудио элемент.
                        </audio>
                      </div>
                      ))}
                    </div>
                  ) : (
                    <p className="no-content">Аудиозаписи не загружены</p>
                  )}
                </div>

                {/* Секция видео */}
                <div className="portfolio-section">
                  <h3>Видеозаписи</h3>
                  {mediaData?.video?.length > 0 ? (
                    <div className="videos-grid">
                      {mediaData.video.map((video) => (
                      <div key={video.id} className="video-item">
                        <video controls src={video.fileUrl} className="portfolio-video" />
                        {video.title && <p className="video-title">{video.title}</p>}
                      </div>
                    ))}
                    </div>
                  ) : (
                    <p className="no-content">Видеозаписи не загружены</p>
                  )}
                </div>
              </div>
            )}

            {activeTab === 'contacts' && (
              <div className="tab-content">
                <div className="contacts-grid">
                  <div className="contact-item">
                    <span className="contact-label">Телефон:</span>
                    <span className="contact-value">{profileData.phone || 'Не указан'}</span>
                  </div>
                  <div className="contact-item">
                    <span className="contact-label">Telegram:</span>
                    <span className="contact-value">{profileData.telegram || 'Не указан'}</span>
                  </div>
                </div>
              </div>
            )}
          </div>
        </div>
      </div>
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