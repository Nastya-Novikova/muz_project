import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import Header from '../../components/Header/Header';
import './ProfilePage.css';

function ProfilePage() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState('info');

  if (!user) {
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

  return (
    <>
      <Header />
      <div className="profile-page">
        <div className="profile-container">
          {/* Шапка профиля */}
          <div className="profile-header">
            <div className="profile-main-info">
              <img src={user.avatar} alt={user.name} className="profile-avatar" />
              <div className="profile-info">
                <h1>{user.fullName || 'Не указано'}</h1>
                <p className="profile-email">{user.email}</p>
                <p className="profile-activity">
                  {user.age && `${user.age} лет`}
                  {user.activityType ? ` - ${user.activityType}` : 'Вид деятельности не указан'}
                  {user.experience && ` - Стаж: ${user.experience}`}
                </p>
                <div className="profile-genres">
                  {user.genres?.map(genre => (
                    <span key={genre} className="genre-tag">{genre}</span>
                  ))}
                </div>
              </div>
            </div>
            <button onClick={handleEditProfile} className="edit-profile-btn">
              <img 
                src='/pencil.png'
                alt='Редактировать'
                className='edit-profile-btn-img'/>
            </button>
          </div>

          {/* Страницы */}
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

          {/* Наполнение страниц */}
          <div className="profile-content">
            {activeTab === 'info' && (
              <div className="tab-content">
                <div className="info-section">
                  <h3>О себе</h3>
                  <p className="profile-description">
                    {user.description || 'Пользователь не добавил информацию о себе'}
                  </p>
                </div>
                
                <div className="info-grid">
                  <div className="info-item">
                    <span className="info-label">Возраст:</span>
                    <span className="info-value">{user.age || 'Не указан'}</span>
                  </div>
                  <div className="info-item">
                    <span className="info-label">Вид деятельности:</span>
                    <span className="info-value">{user.activityType || 'Не указан'}</span>
                  </div>
                  <div className="info-item">
                    <span className="info-label">Стаж:</span>
                    <span className="info-value">
                      {user.experience ? `${user.experience}` : 'Не указан'}
                    </span>
                  </div>
                  <div className="info-item">
                    <span className="info-label">Жанры:</span>
                    <span className="info-value">
                      {user.genres?.length > 0 ? user.genres.join(', ') : 'Не указаны'}
                    </span>
                  </div>
                </div>
              </div>
            )}

            {activeTab === 'portfolio' && (
              <div className="tab-content">
                <div className="portfolio-section">
                  <h3>Аудиозаписи</h3>
                  {user.portfolio?.audio?.length > 0 ? (
                    <div className="audio-list">
                      {user.portfolio.audio.map((audio, index) => (
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
                  {user.portfolio?.photos?.length > 0 ? (
                    <div className="photos-grid">
                      {user.portfolio.photos.map((photo, index) => (
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
                    {user.portfolio?.other || 'Дополнительная информация не добавлена'}
                  </p>
                </div>
              </div>
            )}

            {activeTab === 'contacts' && (
              <div className="tab-content">
                <div className="contacts-grid">
                  <div className="contact-item">
                    <span className="contact-label">Email:</span>
                    <span className="contact-value">{user.email}</span>
                  </div>
                  <div className="contact-item">
                    <span className="contact-label">Телефон:</span>
                    <span className="contact-value">{user.phone || 'Не указан'}</span>
                  </div>
                  <div className="contact-item">
                    <span className="contact-label">Telegram:</span>
                    <span className="contact-value">{user.telegram || 'Не указан'}</span>
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