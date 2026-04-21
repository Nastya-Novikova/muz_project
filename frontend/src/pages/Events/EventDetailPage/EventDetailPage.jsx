// frontend/src/pages/Events/EventDetailPage.jsx
import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAuth } from '../../../context/AuthContext';
import Header from '../../../components/Header/Header';
import { api } from '../../../services/api';
import './EventDetailPage.css';

function EventDetailPage() {
  const { eventId } = useParams();
  const navigate = useNavigate();
  const { getToken } = useAuth();
  
  const [event, setEvent] = useState(null);
  const [loading, setLoading] = useState(true);
  const [actionLoading, setActionLoading] = useState(false);
  const [error, setError] = useState('');
  const [currentUserId, setCurrentUserId] = useState(null);

  const token = getToken();

  useEffect(() => {
    const loadData = async () => {
      setLoading(true);
      try {
        // Получаем профиль текущего пользователя
        const myProfile = await api.getProfile(token);
        setCurrentUserId(myProfile.id);

        // Получаем мероприятие
        const eventData = await api.getEventById(eventId, token);
        setEvent(eventData);
      } catch (err) {
        console.error('Ошибка загрузки:', err);
        setError('Не удалось загрузить мероприятие.');
      } finally {
        setLoading(false);
      }
    };
    
    if (token) {
      loadData();
    }
  }, [eventId, token]);

  const handleBack = () => {
    navigate('/events');
  };

  const handleRegister = async () => {
    setActionLoading(true);
    try {
      await api.registerForEvent(eventId, token);
      const updatedEvent = await api.getEventById(eventId, token);
      setEvent(updatedEvent);
    } catch (err) {
      alert('Не удалось записаться на мероприятие: ' + err.message);
    } finally {
      setActionLoading(false);
    }
  };

  const handleUnregister = async () => {
    setActionLoading(true);
    try {
      await api.unregisterFromEvent(eventId, token);
      const updatedEvent = await api.getEventById(eventId, token);
      setEvent(updatedEvent);
    } catch (err) {
      alert('Не удалось отменить запись: ' + err.message);
    } finally {
      setActionLoading(false);
    }
  };

  const handleEdit = () => {
    navigate(`/events/${eventId}/edit`);
  };

  const handleCancelEvent = async () => {
    if (!window.confirm('Вы уверены, что хотите отменить мероприятие? Это действие необратимо.')) {
      return;
    }
    setActionLoading(true);
    try {
      await api.cancelEvent(eventId, token);
      navigate('/events');
    } catch (err) {
      alert('Не удалось отменить мероприятие: ' + err.message);
    } finally {
      setActionLoading(false);
    }
  };

  const handleCreatorClick = () => {
    if (event?.creatorProfileId) {
      navigate(`/profile/${event.creatorProfileId}`);
    }
  };

  const formatShortDate = (dateString) => {
    const options = { day: 'numeric', month: 'long', year: 'numeric' };
    return new Date(dateString).toLocaleString('ru-RU', options);
  };

  const formatTime = (dateString) => {
    const options = { hour: '2-digit', minute: '2-digit' };
    return new Date(dateString).toLocaleString('ru-RU', options);
  };

  const getStatusText = (status) => {
    switch (status) {
      case 'Scheduled': return 'Запланировано';
      case 'Cancelled': return 'Отменено';
      case 'Completed': return 'Завершено';
      default: return status;
    }
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

  if (loading) {
    return (
      <>
        <Header />
        <div className="event-detail-page">
          <div className="event-detail-container">
            <div className="loading-spinner">Загрузка...</div>
          </div>
        </div>
      </>
    );
  }

  if (error || !event) {
    return (
      <>
        <Header />
        <div className="event-detail-page">
          <div className="event-detail-container">
            <p>Ошибка: {error || 'Мероприятие не найдено'}</p>
            <button onClick={handleBack} className="back-btn-events">Назад</button>
          </div>
        </div>
      </>
    );
  }

  const isCreator = event.creatorProfileId === currentUserId;
  const canRegister = !isCreator && event.status === 'Scheduled';
  const isFull = event.maxParticipants > 0 && event.currentParticipants >= event.maxParticipants;

  return (
    <>
      <Header />
      <div className="event-detail-page">
        <div className="event-detail-container">
          {/* Шапка с изображением */}
          {event.imageUrl ? (
            <div className="event-header">
              <img 
                src={event.imageUrl} 
                alt={event.title} 
                className="event-header-image"
                onError={(e) => {

                  e.target.style.display = 'none';
                  e.target.parentElement.classList.add('placeholder-bg');
                  e.target.parentElement.style.backgroundColor = generatePastelColor(event.id);
                }}
              />
            </div>
          ) : (
            <div 
              className="event-header placeholder-bg"
              style={{ backgroundColor: generatePastelColor(event.id) }}
            />
          )}

          {/* Контент */}
          <div className="event-content">

            {/* Заголовок и статус */}
            <div className="event-title-section">
              <h1 className="event-detail-title">{event.title}</h1>
              <span className={`status-badge ${event.status?.toLowerCase() || ''}`}>
                {getStatusText(event.status)}
              </span>
            </div>

            {/* Основная информация */}
            <div className="info-section">
              <h3>Основная информация</h3>
              
              <div className="info-grid-events">
                <div className="info-item">
                  <span className="info-label">Организатор:</span>
                  <span 
                    className="info-value clickable" 
                    onClick={handleCreatorClick}
                  >
                    {event.creatorFullName || 'Не указан'}
                  </span>
                </div>

                <div className="info-item">
                  <span className="info-label">Регион:</span>
                  <span className="info-value">{event.region?.localizedName || 'Не указан'}</span>
                </div>

                <div className="info-item">
                  <span className="info-label">Город:</span>
                  <span className="info-value">{event.city?.localizedName || 'Не указан'}</span>
                </div>

                <div className="info-item">
                  <span className="info-label">Адрес:</span>
                  <span className="info-value">{event.address || 'Не указан'}</span>
                </div>

                <div className="info-item">
                  <span className="info-label">Дата:</span>
                  <span className="info-value">{formatShortDate(event.startDateTime)}</span>
                </div>

                <div className="info-item">
                  <span className="info-label">Время начала:</span>
                  <span className="info-value">{formatTime(event.startDateTime)}</span>
                </div>

                {event.endDateTime && (
                  <div className="info-item">
                    <span className="info-label">Время окончания:</span>
                    <span className="info-value">{formatTime(event.endDateTime)}</span>
                  </div>
                )}

                <div className="info-item">
                  <span className="info-label">Участники:</span>
                  <span className="info-value">
                    {event.currentParticipants} 
                    {event.maxParticipants > 0 ? ` / ${event.maxParticipants}` : ' (без ограничений)'}
                  </span>
                </div>
              </div>
            </div>

            {/* Описание */}
            {event.description && (
              <div className="info-section">
                <h3>Описание</h3>
                <p className="event-description">{event.description}</p>
              </div>
            )}

            {/* Действия */}
            <div className="event-actions-section">
              <button onClick={handleBack} className="back-btn-events">
                Назад
              </button>
              
              {isCreator ? (
                <>
                  <button onClick={handleEdit} className="edit-btn" disabled={actionLoading}>
                    Редактировать
                  </button>
                  {event.status === 'Scheduled' && (
                    <button onClick={handleCancelEvent} className="cancel-event-btn" disabled={actionLoading}>
                      Отменить мероприятие
                    </button>
                  )}
                </>
              ) : (
                canRegister && (
                  event.isRegistered ? (
                    <button onClick={handleUnregister} className="unregister-btn" disabled={actionLoading}>
                      Отменить участие
                    </button>
                  ) : (
                    <button 
                      onClick={handleRegister} 
                      className="register-btn" 
                      disabled={actionLoading || isFull}
                    >
                      {isFull ? 'Мест нет' : 'Записаться'}
                    </button>
                  )
                )
              )}
            </div>
          </div>
        </div>
      </div>
    </>
  );
}

export default EventDetailPage;