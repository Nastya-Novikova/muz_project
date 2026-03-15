import React from 'react';
import { api } from '../../services/api'; 
import './UserCard.css';
import { useNavigate } from 'react-router-dom';

const UserCard = ({ user, onProfileClick }) => {
  const navigate = useNavigate();

  const transformUserData = (userData) => {
    if (!userData) return null;
    
    return {
      id: userData.id,
      fullName: userData.fullName || 'Не указано',
      age: userData.age || '',
      city: userData.city?.localizedName || 'Не указан',
      avatar: api.getAvatarUrl(userData.avatarUrl) || `/default-avatar.png`,
      activityType: userData.specialties?.map(s => s.localizedName || s.name) || [],
      genres: userData.genres?.map(g => g.localizedName || g.name) || [],
      experience: userData.experience || 0,
      description: userData.description || 'Нет описания',
      profileType: userData.profileType, 
      lookingFor: userData.lookingFor
    };
  };

  const transformedUser = transformUserData(user);

  const handleProfileClick = () => {
    if (onProfileClick) {
      onProfileClick(transformedUser.id);
    } else {
      navigate(`/profile/${transformedUser.id}`);
    }
  };

  return (
    <div className='user-card'>
      {/* Шапка карточки */}
      <div className="card-header" onClick={handleProfileClick} style={{ cursor: 'pointer' }}>
        <div className="photo-container">
          <img src={transformedUser.avatar} alt={transformedUser.fullName} className="user-photo" />
        </div>
        
        <div className="user-main-info">
          <h3 className="user-name">{transformedUser.fullName}</h3>
          <div className="user-meta">
            <span className="user-role">{transformedUser.profileType === "Band" ? 'Коллектив' : 'Музыкант'} • </span>
            <span className="user-age">{transformedUser.age} {transformedUser.profileType === "Band" ? 'год' : 'лет'} • </span>
            <span className="user-location">{transformedUser.city}</span>
          </div>
        </div>
      </div>

      {/* Основная информация */}
      <div className="card-body">
        <div className="info-container">
          <div className="info-row">
            <span className="info-label-card">Деятельность:</span>
            <span className="info-value-card">
              {transformedUser.activityType.slice(0, 2).join(', ')}
              {transformedUser.activityType.length > 2 && '...'}
            </span>
          </div>
          
          <div className="info-row">
            <span className="info-label-card">Жанры:</span>
            <span className="info-value-card">
              {transformedUser.genres.slice(0, 2).join(', ')}
              {transformedUser.genres.length > 2 && '...'}
            </span>
          </div>
          
          <div className="info-row">
            <span className="info-label-card">Стаж:</span>
            <span className="info-value-card">{transformedUser.experience} {transformedUser.experience ? '': ''}</span>
          </div>

          {transformedUser.description && (
            <div className="description">
              {transformedUser.description.length > 100 
                ? transformedUser.description.substring(0, 100) + '...' 
                : transformedUser.description}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default UserCard;