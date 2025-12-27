// src/pages/Profile/ProfilePage.jsx
import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { api } from '../../services/api'; // Импортируем api
import Header from '../../components/Header/Header';
import './ProfilePage.css';

function ProfilePage() {
const { userId } = useParams();
  const { getToken, logout, getUserEmail } = useAuth(); // logout для случая ошибки 401
  const navigate = useNavigate();
  
  const [activeTab, setActiveTab] = useState('info');
  const [profileData, setProfileData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const loadProfile = async () => {
      setLoading(true);
      setError('');
      
      try {
        const token = getToken();
        
        if (!userId) {
          // Загружаем свой профиль
          const data = await api.getProfile(token);
          setProfileData(data);
        } else {
          const data = await api.getProfileById(userId, token);
          setProfileData(data);
        }
      } catch (err) {
        console.error('Ошибка загрузки профиля:', err);
        
        if (err.message.includes('401') || err.message.includes('Unauthorized')) {
          // Неавторизован - выходим
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

  const isOwnProfile = !userId;
  const userEmail = getUserEmail();

  const handleEditProfile = () => {
    navigate('/profile/edit');
  };

  const handleBack = () => {
    navigate(-1);
  };

  /*const handleToggleFavorite = () => {
    // TODO: Реализовать логику добавления/удаления из избранного
    // const token = getToken();
    // if (isFavorite) {
    //   await api.removeFromFavorites(userId, token);
    // } else {
    //   await api.addToFavorites(userId, token);
    // }
    setIsFavorite(!isFavorite);
  };*/

  /*const handleCollaboration = () => {
    // TODO: Реализовать логику отправки предложения сотрудничества
    // const token = getToken();
    // await api.sendCollaborationRequest(userId, token);
    setIsCollaborationSent(true);
  };*/

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
                src={'/default-avatar.png'} //profileData.avatarUrl || 
                alt={profileData.fullName}
                className="profile-avatar" 
              />
              <div className="profile-info">
                <h1>{profileData.fullName || 'Не указано'}</h1>
                {isOwnProfile && userEmail && (
                  <p className="profile-email">{userEmail}</p>
                )}
                <p className="profile-activity">
                  {profileData.age && `${profileData.age} лет`}
                  {profileData.specialties?.[0]?.localizedName && ` • ${profileData.specialties[0].localizedName}`}
                  {profileData.experience && ` • Стаж: ${profileData.experience}`}
                  {profileData.city?.localizedName && ` • ${profileData.city.localizedName}`}
                </p>
                <div className="profile-genres">
                  {profileData.genres?.map(genre => (
                    <span key={genre.id} className="genre-tag">
                      {genre.localizedName || genre.name}
                    </span>
                  ))}
                </div>
              </div>
            </div>

            <div className="profile-actions">
              {isOwnProfile ? (
                <button onClick={handleEditProfile} className="edit-profile-btn">
                  <img
                    src='/pencil.png'
                    alt='Редактировать профиль'
                    className='edit-profile-btn-img'
                  />
                </button>
              ) : (
                <div className="profile-actions-btn">
                  <button
                    className={`favorite-profile-btn`}
                  >
                    {'В избранное'}
                  </button>
                  <button onClick={handleBack} className="back-btn">
                    Назад
                  </button>
                  <button
                    className={`collaboration-btn`}
                  >
                    {'Предложить сотрудничество'}
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
                    <span className="info-label">Возраст:</span>
                    <span className="info-value">{profileData.age || 'Не указан'}</span>
                  </div>
                  <div className="info-item">
                    <span className="info-label">Город:</span>
                    <span className="info-value">{profileData.city?.localizedName || 'Не указан'}</span>
                  </div>
                  <div className="info-item">
                    <span className="info-label">Вид деятельности:</span>
                    <span className="info-value">
                      {profileData.specialties?.map(s => s.localizedName).join(', ') || 'Не указан'}
                    </span>
                  </div>
                  <div className="info-item">
                    <span className="info-label">Стаж:</span>
                    <span className="info-value">
                      {profileData.experience ? `${profileData.experience} лет` : 'Не указан'}
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

            {activeTab === 'portfolio' && (
              <div className="tab-content">
                <div className="portfolio-section">
                  <h3>Аудиозаписи</h3>
                  {profileData.portfolio?.audio?.length > 0 ? (
                    <div className="audio-list">
                      {profileData.portfolio.audio.map((audio, index) => (
                        <div key={index} className="audio-item">
                          <audio controls src={audio.url}>
                            Ваш браузер не поддерживает аудио элемент.
                          </audio>
                          <p>{audio.title || `Аудиозапись ${index + 1}`}</p>
                        </div>
                      ))}
                    </div>
                  ) : (
                    <p className="no-content">Аудиозаписи не загружены</p>
                  )}
                </div>

                <div className="portfolio-section">
                  <h3>Фотографии и сертификаты</h3>
                  {profileData.portfolio?.photos?.length > 0 ? (
                    <div className="photos-grid">
                      {profileData.portfolio.photos.map((photo, index) => (
                        <img 
                          key={index} 
                          src={photo.url} 
                          alt={photo.title || `Фото ${index + 1}`}
                          className="portfolio-photo"
                        />
                      ))}
                    </div>
                  ) : (
                    <p className="no-content">Фотографии не загружены</p>
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
    </>
  );
}

export default ProfilePage;