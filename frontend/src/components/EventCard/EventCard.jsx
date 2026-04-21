import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { api } from '../../services/api';
import './EventCard.css';

const EventCard = ({ event }) => {
  const navigate = useNavigate();
  const [imageError, setImageError] = useState(false);

  const handleClick = () => {
    const url = `/events/${event.id}`;
    window.open(url, '_blank');
  };

  const formatDate = (dateString) => {
    const options = { day: 'numeric', month: 'long', hour: '2-digit', minute: '2-digit' };
    return new Date(dateString).toLocaleString('ru-RU', options);
  };

  const generatePastelColor = (str) => {
    let hash = 0;
    for (let i = 0; i < str.length; i++) {
      hash = str.charCodeAt(i) + ((hash << 5) - hash);
    }
    
    const h = Math.abs(hash) % 360;
    const s = 40 + (Math.abs(hash) % 30);
    const l = 75 + (Math.abs(hash) % 15); 
    
    return `hsl(${h}, ${s}%, ${l}%)`;
  };

  const truncateText = (text, maxLength) => {
    if (!text) return '';
    if (text.length <= maxLength) return text;
    return text.substring(0, maxLength) + '...';
  };

  return (
    <div className="event-card" onClick={handleClick}>
      {event.imageUrl && !imageError ? (
        <img 
          src={event.imageUrl} 
          alt={event.title} 
          className="event-image"
          onError={(e) => {

                  e.target.style.display = 'none';
                  e.target.parentElement.classList.add('event-image-placeholder');
                  e.target.parentElement.style.backgroundColor = generatePastelColor(event.id);
                }}
        />
      ) : (
        <div 
          className="event-image-placeholder"
          style={{ backgroundColor: generatePastelColor(event.id)}}
        />
      )}

      <div className="event-content">
        <h3 className="event-title" title={`${event.title}`}>
          {truncateText(`${event.title}`, 22)}
        </h3>
        
        <div className="event-info">
          <div className="event-info-row">
            <span className="event-info-icon">📍</span>
            <span className="event-info-text" title={`${event.city?.localizedName}, ${event.address}`}>
              {truncateText(`${event.city?.localizedName}, ${event.address}`, 30)}
            </span>
          </div>
          <div className="event-info-row">
            <span className="event-info-icon">📅</span>
            {formatDate(event.startDateTime)}
          </div>
        </div>

        {event.description && (
          <p className="event-description" title={event.description}>
            {truncateText(event.description, 65)}
          </p>
        )}

        <div className="event-footer">
          <div className="event-creator">
            <img 
              src={api.getAvatarUrl(event.creatorAvatarUrl)} 
              alt={event.creatorFullName} 
              className="creator-avatar"
            />
            <span className="creator-name">{event.creatorFullName}</span>
          </div>
          <div className="event-participants">
            {event.currentParticipants} / {event.maxParticipants}
          </div>
        </div>
      </div>
    </div>
  );
};

export default EventCard;