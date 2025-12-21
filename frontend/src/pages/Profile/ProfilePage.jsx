import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import Header from '../../components/Header/Header';
import './ProfilePage.css';

function ProfilePage() {
  const { userId } = useParams();
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState('info');
  const [isFavorite, setIsFavorite] = useState(false);
  const [profileUser, setProfileUser] = useState(null);
  const [isCollaborationSent, setIsCollaborationSent] = useState(false);

  const mockOtherUsers = {
    '2': {
      id: '2',
      fullName: 'Смирнова Анна Дмитриевна',
      age: 26,
      city: 'Москва',
      avatar: 'https://ui-avatars.com/api/?name=Анна+Смирнова&background=f56565',
      activityType: 'Вокал',
      genres: ['Поп', 'Джаз', 'Соул'],
      experience: 4,
      description: 'Джазовая вокалистка, выпускница музыкального колледжа. Ищу бэнд для выступлений в клубах и на мероприятиях.',
      email: 'anna.smirnova@example.com',
      phone: '+7 (999) 987-65-43',
      telegram: '@anna_vocal',
      portfolio: {
        audio: ['vocal_demo.mp3'],
        photos: [],
        other: 'Участвовала в джазовых фестивалях Москвы'
      }
    }
  };

  useEffect(() => {
    if (userId) {
      const otherUser = mockOtherUsers[userId] || mockOtherUsers['2'];
      setProfileUser(otherUser);
    
      const favorites = JSON.parse(localStorage.getItem('musicianFinder_favorites') || '[]');
      setIsFavorite(favorites.includes(userId));
      
      const collaborations = JSON.parse(localStorage.getItem('musicianFinder_collaborations') || '[]');
      setIsCollaborationSent(collaborations.includes(userId));
    } else {
      setProfileUser(user);
    }
  }, [userId, user]);

  const isOwnProfile = !userId;

  if (!user && !userId) {
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
    if (!userId) return;
    
    const favorites = JSON.parse(localStorage.getItem('musicianFinder_favorites') || '[]');
    
    if (isFavorite) {

      const newFavorites = favorites.filter(id => id !== userId);
      localStorage.setItem('musicianFinder_favorites', JSON.stringify(newFavorites));
      setIsFavorite(false);
    } else {

      favorites.push(userId);
      localStorage.setItem('musicianFinder_favorites', JSON.stringify(favorites));
      setIsFavorite(true);
    }
  };

  const handleCollaboration = () => {
  if (!userId || isCollaborationSent) return;

  const collaborations = JSON.parse(localStorage.getItem('musicianFinder_collaborations') || '[]');
  collaborations.push(userId);
  localStorage.setItem('musicianFinder_collaborations', JSON.stringify(collaborations));
  
  setIsCollaborationSent(true);

  console.log('Предложение сотрудничества отправлено пользователю:', userId);
};

  if (!profileUser) {
    return (
      <>
        <Header />
        <div className="profile-page">
          <div className="profile-container">
            <p>Пользователь не найден</p>
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
                          <audio controls src={audio} className="audio-player">
                            Ваш браузер не поддерживает аудио элемент.
                          </audio>
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
                        <div key={index} className="photo-item">
                          <img src={photo} alt={`Фото ${index + 1}`} />
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