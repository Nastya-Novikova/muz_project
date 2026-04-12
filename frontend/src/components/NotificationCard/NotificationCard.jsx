import React from 'react';
import './NotificationCard.css';

const NotificationCard = ({ notification, onMarkAsRead }) => {
  // Определяем букву для иконки по типу уведомления
  const getIconLetter = () => {
    switch (notification.type) {
      case 'EventRegistration':
        return 'М';
      case 'EventReminder':
        return 'М'; // Мероприятие
      case 'CollaborationReceived':
        return 'П'; // Предложение
      default:
        return '!';
    }
  };

  const formatDate = (dateString) => {
    const date = new Date(dateString);
    const now = new Date();
    const diffMs = now - date;
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);
    
    if (diffMins < 1) return 'Только что';
    if (diffMins < 60) return `${diffMins} мин назад`;
    if (diffHours < 24) return `${diffHours} ч назад`;
    if (diffDays < 7) return `${diffDays} дн назад`;
    return date.toLocaleDateString('ru-RU');
  };

  const handleClick = () => {
    if (!notification.isRead && onMarkAsRead) {
      onMarkAsRead(notification.id);
    }

    switch (notification.type) {
      case 'CollaborationReceived':
        navigate('/suggestions', { state: { activeTab: 'received' } });
        break;
      
      /*case 'EventRegistration':
      case 'EventReminder':
        if (notification.entityId) {
          navigate(`/events/${notification.entityId}`);
        }
        break;*/
      
      default:
        break;
    }
  };

  return (
    <div 
      className={`notification-card ${notification.isRead ? 'read' : 'unread'}`}
      onClick={handleClick}
    >
      <div className="notification-icon">
        {getIconLetter()}
      </div>
      <div className="notification-content">
        <div className="notification-header">
          <h4 className="notification-title">{notification.title}</h4>
          <span className="notification-date">{formatDate(notification.createdAt)}</span>
        </div>
        {notification.message && (
          <p className="notification-message">{notification.message}</p>
        )}
      </div>
    </div>
  );
};

export default NotificationCard;