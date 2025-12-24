import React from 'react';
import './UserCard.css';
import { useNavigate } from 'react-router-dom';

const UserCard = ({ 
  user,
  showActions = true,
  onFavoriteClick,
  onProfileClick,
  isFavorite = false
}) => {
  const userData = user || {
    id: '1',
    fullName: 'Иванов Иван Иванович',
    age: 28,
    avatar: 'https://ui-avatars.com/api/?name=Иван+Иванов&background=random',
    activityType: 'Гитара, Вокал',
    genres: ['Рок', 'Джаз', 'Блюз'],
    experience: 5,
    description: 'Опытный музыкант с 5-летним стажем. Ищу коллектив для создания рок-группы...',
    location: 'Москва'
  };

  const navigate = useNavigate();

  const handleProfileClick = () => {
    if (onProfileClick) {
      onProfileClick(userData.id);
    } else {
      navigate(`/profile/${userData.id}`);
    }
  };

  const handleFavorite = () => {
    if (onFavoriteClick) {
      onFavoriteClick(userData.id);
    }
  };
  
  return (
    <div className='user-card'>
      {/* Шапка карточки */}
      <div className="card-header" onClick={handleProfileClick} style={{ cursor: 'pointer' }}>
        <div className="photo-container">
          <img src={userData.avatar} alt={userData.fullName} className="user-photo" />
        </div>
        
        <div className="user-main-info">
          <h3 className="user-name">{userData.fullName}</h3>
          <div className="user-meta">
            <span className="user-age">{userData.age} лет</span>
            <span className="user-divider">•</span>
            <span className="user-location">{userData.city || 'Не указан'}</span>
          </div>
        </div>
      </div>

      {/* Основная информация */}
      <div className="card-body">
        <div className="info-container">
          <div className="info-row">
            <span className="info-label-card">Деятельность:</span>
            <span className="info-value-card">{userData.activityType}</span>
          </div>
          
          <div className="info-row">
            <span className="info-label-card">Жанры:</span>
            <span className="info-value-card">
              {userData.genres?.slice(0, 2).join(', ')}
            </span>
          </div>
          
          <div className="info-row">
            <span className="info-label-card">Стаж:</span>
            <span className="info-value-card">{userData.experience} лет</span>
          </div>

          <div className="description">
              {userData.description}
          </div>
        </div>
      </div>
    </div>
  );
};

export default UserCard;