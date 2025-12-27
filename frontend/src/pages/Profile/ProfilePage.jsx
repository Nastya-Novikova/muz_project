// src/pages/Profile/ProfilePage.jsx
import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { api } from '../../services/api'; // Импортируем api
import Header from '../../components/Header/Header';
import './ProfilePage.css';

function ProfilePage() {
  const { userId } = useParams(); // userId - для просмотра чужого профиля
  const { user: currentUser, getToken } = useAuth(); // currentUser - данные текущего пользователя из AuthContext
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState('info');
  const [isFavorite, setIsFavorite] = useState(false);
  const [profileUser, setProfileUser] = useState(null); // Данные профиля для отображения
  const [isCollaborationSent, setIsCollaborationSent] = useState(false);
  const [loading, setLoading] = useState(true); // Состояние загрузки
  const [error, setError] = useState(''); // Состояние ошибки

  useEffect(() => {
    const loadProfile = async () => {
      setLoading(true);
      setError('');
      try {
        if (!userId) {
          // Просмотр своего профиля - используем данные из AuthContext
          setProfileUser(currentUser);
          // Состояния для своего профиля можно сбросить или не отображать
          setIsFavorite(false);
          setIsCollaborationSent(false);
        } else {
          // Просмотр чужого профиля - загружаем с бэкенда
          const token = getToken();
          // TODO: Вызовите метод API для получения профиля по userId
          // const otherUserProfile = await api.getProfileById(userId, token); // Пример метода
          // setProfileUser(otherUserProfile);

          // Заглушка: пока нет метода getProfileById
          setError('Просмотр чужого профиля пока не реализован');
          setProfileUser(null);
        }
      } catch (err) {
        console.error('Ошибка при загрузке профиля:', err);
        setError(err.message || 'Не удалось загрузить профиль');
        setProfileUser(null);
      } finally {
        setLoading(false);
      }
    };

    loadProfile();
  }, [userId, currentUser, getToken]); // Зависимости

  const isOwnProfile = !userId;

  if (!currentUser && !userId) {
    return (
      <>
        <Header />
        <div className="profile-page">
          <div className="profile-container">
            <p>Пожалуйста, войдите в систему</p>
          </div>
        </div>
      </>
    );
  }

  const handleEditProfile = () => {
    navigate('/profile/edit');
  };

  const handleBack = () => {
    navigate(-1);
  };

  const handleToggleFavorite = () => {
    // TODO: Реализовать логику добавления/удаления из избранного
    // const token = getToken();
    // if (isFavorite) {
    //   await api.removeFromFavorites(userId, token);
    // } else {
    //   await api.addToFavorites(userId, token);
    // }
    setIsFavorite(!isFavorite);
  };

  const handleCollaboration = () => {
    // TODO: Реализовать логику отправки предложения сотрудничества
    // const token = getToken();
    // await api.sendCollaborationRequest(userId, token);
    setIsCollaborationSent(true);
  };

  if (loading) {
    return (
      <>
        <Header />
        <div className="profile-page">
          <div className="profile-container">
            <p>Загрузка...</p>
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

  if (!profileUser) {
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
              <img src={profileUser.avatar} alt={profileUser.fullName} className="profile-avatar" />
              <div className="profile-info">
                <h1>{profileUser.fullName || 'Не указано'}</h1>
                <p className="profile-email">{profileUser.email}</p>
                <p className="profile-activity">
                  {profileUser.age && `${profileUser.age} лет`}
                  {profileUser.activityType && ` • ${profileUser.activityType}`}
                  {profileUser.experience && ` • Стаж: ${profileUser.experience}`}
                  {profileUser.city && ` • ${profileUser.city}`}
                </p>
                <div className="profile-genres">
                  {profileUser.genres?.map(genre => (
                    <span key={genre} className="genre-tag">{genre}</span>
                  ))}
                </div>
              </div>
            </div>

            {/* Кнопки действий */}
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
                    onClick={handleToggleFavorite}
                    className={`favorite-profile-btn ${isFavorite ? 'active' : ''}`}
                  >
                    {isFavorite ? 'В избранном' : 'В избранное'}
                  </button>
                  <button onClick={handleBack} className="back-btn">
                    Назад
                  </button>
                  <button
                    onClick={handleCollaboration}
                    className={`collaboration-btn ${isCollaborationSent ? 'sent' : ''}`}
                    disabled={isCollaborationSent}
                  >
                    {isCollaborationSent ? 'Предложение направлено' : 'Предложить сотрудничество'}
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
                    {profileUser.description || 'Пользователь не добавил информацию о себе'}
                  </p>
                </div>

                <div className="info-grid">
                  <div className="info-item">
                    <span className="info-label">Возраст:</span>
                    <span className="info-value">{profileUser.age || 'Не указан'}</span>
                  </div>
                  <div className="info-item">
                    <span className="info-label">Город:</span>
                    <span className="info-value">{profileUser.city || 'Не указан'}</span>
                  </div>
                  <div className="info-item">
                    <span className="info-label">Вид деятельности:</span>
                    <span className="info-value">{profileUser.activityType || 'Не указан'}</span>
                  </div>
                  <div className="info-item">
                    <span className="info-label">Стаж:</span>
                    <span className="info-value">
                      {profileUser.experience ? `${profileUser.experience} лет` : 'Не указан'}
                    </span>
                  </div>
                  <div className="info-item">
                    <span className="info-label">Жанры:</span>
                    <span className="info-value">
                      {profileUser.genres?.length > 0 ? profileUser.genres.join(', ') : 'Не указаны'}
                    </span>
                  </div>
                </div>
              </div>
            )}

            {activeTab === 'portfolio' && (
              <div className="tab-content">
                <div className="portfolio-section">
                  <h3>Аудиозаписи</h3>
                  {profileUser.portfolio?.audio?.length > 0 ? (
                    <div className="audio-list">
                      {profileUser.portfolio.audio.map((audio, index) => (
                        <div key={index} className="audio-item">
                          <span>Аудиозапись {index + 1}</span>
                          {/* Для воспроизведения аудио с бэкенда нужно будет получить URL */}
                          {/* <audio controls src={audio.url}> */}
                          {/*   Ваш браузер не поддерживает аудио элемент. */}
                          {/* </audio> */}
                          <p>Файл: {audio.filename || `audio_${index + 1}`}</p>
                        </div>
                      ))}
                    </div>
                  ) : (
                    <p className="no-content">Аудиозаписи не загружены</p>
                  )}
                </div>

                <div className="portfolio-section">
                  <h3>Фотографии и сертификаты</h3>
                  {profileUser.portfolio?.photos?.length > 0 ? (
                    <div className="photos-grid">
                      {profileUser.portfolio.photos.map((photo, index) => (
                        // Для отображения фото с бэкенда нужно будет получить URL
                        // <img key={index} src={photo.url} alt={`Фото ${index + 1}`} />
                        <div key={index} className="photo-item">
                          <p>Фото: {photo.filename || `photo_${index + 1}`}</p>
                        </div>
                      ))}
                    </div>
                  ) : (
                    <p className="no-content">Фотографии не загружены</p>
                  )}
                </div>

                <div className="portfolio-section">
                  <h3>Дополнительная информация</h3>
                  <p className="portfolio-other">
                    {profileUser.portfolio?.other || 'Дополнительная информация не добавлена'}
                  </p>
                </div>
              </div>
            )}

            {activeTab === 'contacts' && (
              <div className="tab-content">
                <div className="contacts-grid">
                  <div className="contact-item">
                    <span className="contact-label">Email:</span>
                    <span className="contact-value">{profileUser.email}</span>
                  </div>
                  <div className="contact-item">
                    <span className="contact-label">Телефон:</span>
                    <span className="contact-value">{profileUser.phone || 'Не указан'}</span>
                  </div>
                  <div className="contact-item">
                    <span className="contact-label">Telegram:</span>
                    <span className="contact-value">{profileUser.telegram || 'Не указан'}</span>
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