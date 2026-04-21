import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { api } from '../../services/api';
import Header from '../../components/Header/Header';
import NotificationCard from '../../components/NotificationCard/NotificationCard';
import './NotificationsPage.css';

function NotificationsPage() {
  const navigate = useNavigate();
  const { getToken, getUserEmail } = useAuth();
  
  const [notifications, setNotifications] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [unreadCount, setUnreadCount] = useState(0);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [loadingMore, setLoadingMore] = useState(false);
  
  // Статусы подключенных уведомлений (из профиля)
  const [notifyByEmail, setNotifyByEmail] = useState(false);
  const [notifyByVk, setNotifyByVk] = useState(false);

  useEffect(() => {
    loadProfileAndNotifications();
  }, []);

  const loadProfileAndNotifications = async () => {
  setLoading(true);
  try {
    const token = getToken();
    if (!token) {
      navigate('/login');
      return;
    }

    const settings = await api.getNotificationSettings(token);
    setNotifyByEmail(settings.notifyByEmail || false);
    setNotifyByVk(settings.notifyByVk || false);

    await loadNotifications(1, true);
    
    const unreadResponse = await api.getUnreadNotificationsCount(token);
    setUnreadCount(unreadResponse.unreadCount || 0);
    
  } catch (err) {
    console.error('Ошибка загрузки:', err);
    setError('Не удалось загрузить данные');
  } finally {
    setLoading(false);
  }
};

  const loadNotifications = async (pageNum, reset = false) => {
    if (loadingMore && !reset) return;
    
    if (!reset) setLoadingMore(true);
    
    try {
      const token = getToken();
      const response = await api.getNotifications(token, pageNum, 20);
      
      if (reset) {
        setNotifications(response.items || []);
      } else {
        setNotifications(prev => [...prev, ...(response.items || [])]);
      }
      
      setTotalPages(response.totalPages || 1);
      setHasMore(pageNum < (response.totalPages || 1));
      setPage(pageNum);
      
    } catch (err) {
      console.error('Ошибка загрузки уведомлений:', err);
      setError('Не удалось загрузить уведомления');
    } finally {
      if (!reset) setLoadingMore(false);
    }
  };

  const handleMarkAsRead = async (notificationId) => {
    try {
      const token = getToken();
      await api.markNotificationAsRead(notificationId, token);
      
      // Обновляем локальное состояние
      setNotifications(prev => 
        prev.map(notif => 
          notif.id === notificationId ? { ...notif, isRead: true } : notif
        )
      );
      
      // Обновляем счетчик непрочитанных
      setUnreadCount(prev => Math.max(0, prev - 1));
      
    } catch (err) {
      console.error('Ошибка отметки уведомления:', err);
    }
  };

  const handleMarkAllAsRead = async () => {
    try {
      const token = getToken();
      await api.markAllNotificationsAsRead(token);
      
      // Обновляем локальное состояние
      setNotifications(prev => 
        prev.map(notif => ({ ...notif, isRead: true }))
      );
      
      setUnreadCount(0);
      
    } catch (err) {
      console.error('Ошибка отметки всех уведомлений:', err);
    }
  };

  const handleLoadMore = () => {
    if (hasMore && !loadingMore) {
      loadNotifications(page + 1);
    }
  };

  if (loading) {
    return (
      <>
        <Header />
        <div className="notifications-page">
          <div className="notifications-container">
            <div className="loading-spinner">Загрузка уведомлений...</div>
          </div>
        </div>
      </>
    );
  }

  return (
    <>
      <Header />
      <div className="notifications-page">
        <div className="notifications-container">
          <div className="notifications-header">
            <h1>Уведомления</h1>
            {unreadCount > 0 && (
              <span className="unread-count">Непрочитанных: {unreadCount}</span>
            )}
          </div>

          {/* Блок статуса подключенных уведомлений */}
          <div className="notifications-status">
            {notifyByEmail && (
              <div className="status-badge email">
                ✓ У вас подключены уведомления на электронную почту
              </div>
            )}
            {notifyByVk && (
              <div className="status-badge vk">
                ✓ У вас подключены уведомления ВКонтакте
              </div>
            )}
            {!notifyByEmail && !notifyByVk && (
              <div className="status-message">
                Вы можете получать уведомления на{' '}
                <a href="/profile/edit">электронную почту</a> (можно изменить в Управлении профилем) 
                {' '}или в{' '}
                <a href="/suggestions">ВКонтакте</a>
              </div>
            )}
          </div>

          {/* Кнопка "Отметить все" */}
          {unreadCount > 0 && (
            <div className="mark-all-container">
              <button 
                className="mark-all-read-btn"
                onClick={handleMarkAllAsRead}
              >
                Отметить все как прочитанные
              </button>
            </div>
          )}

          {/* Список уведомлений */}
          <div className="notifications-list">
            {notifications.length === 0 ? (
              <div className="empty-state">
                <div className="empty-icon">🔔</div>
                <h3>Нет уведомлений</h3>
                <p>У вас пока нет уведомлений. Когда появятся новые, они будут здесь</p>
              </div>
            ) : (
              <>
                {notifications.map(notification => (
                  <NotificationCard 
                    key={notification.id} 
                    notification={notification}
                    onMarkAsRead={handleMarkAsRead}
                  />
                ))}
                
                {hasMore && (
                  <div className="load-more-container">
                    <button
                      className="load-more-btn"
                      onClick={handleLoadMore}
                      disabled={loadingMore}
                    >
                      {loadingMore ? 'Загрузка...' : 'Загрузить еще'}
                    </button>
                  </div>
                )}
              </>
            )}
          </div>
        </div>
      </div>
    </>
  );
}

export default NotificationsPage;