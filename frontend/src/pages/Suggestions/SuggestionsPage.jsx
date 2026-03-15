import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { api } from '../../services/api';
import Header from '../../components/Header/Header';
import UserCard from '../../components/UserCard/UserCard';
import './SuggestionsPage.css';

function SuggestionsPage() {
  const navigate = useNavigate();
  const { getToken } = useAuth();
  const [activeTab, setActiveTab] = useState('received');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  
  const [receivedUsers, setReceivedUsers] = useState([]);
  const [sentUsers, setSentUsers] = useState([]);
  const [favoriteUsers, setFavoriteUsers] = useState([]);
  const [favoriteIds, setFavoriteIds] = useState(new Set());
  const [tabLoading, setTabLoading] = useState(false);

  useEffect(() => {
    const loadAllData = async () => {
      setLoading(true);
      setError('');
      
      try {
        const token = getToken();
        if (!token) {
          navigate('/login');
          return;
        }

        console.log('=== Начало загрузки всех данных ===');
        const [receivedResponse, sentResponse, favoritesResponse] = await Promise.all([
          api.getReceivedSuggestions(token).catch(err => {
            console.error('Ошибка загрузки полученных предложений:', err);
            return null;
          }),
          api.getSentSuggestions(token).catch(err => {
            console.error('Ошибка загрузки отправленных предложений:', err);
            return null;
          }),
          api.getFavorites(token).catch(err => {
            console.error('Ошибка загрузки избранного:', err);
            return null;
          })
        ]);

        const extractUsers = (response, tabName) => {
          if (!response) return [];
          
          if (tabName === 'favorites' && response.items) {
            return response.items;
          }
          if (tabName === 'received' && response.items) {
            return response.items.map(item => ({
              user: item.fromProfile,
              message: item.message
            }));
          }
          if (tabName === 'sent' && response.items) {
            return response.items.map(item => ({
              user: item.toProfile,
              message: item.message
            }));
          }
          if (Array.isArray(response)) {
            return response;
          }
          return [];
        };

        const received = extractUsers(receivedResponse, 'received');
        const sent = extractUsers(sentResponse, 'sent');
        const favorites = extractUsers(favoritesResponse, 'favorites');

        console.log('Загружено полученных предложений:', received.length);
        console.log('Загружено отправленных предложений:', sent.length);
        console.log('Загружено избранных:', favorites.length);

        setReceivedUsers(received);
        setSentUsers(sent);
        setFavoriteUsers(favorites);
        
        const ids = new Set(favorites.map(user => user.id));
        setFavoriteIds(ids);

      } catch (err) {
        console.error('Общая ошибка загрузки данных:', err);
        setError('Не удалось загрузить данные. Пожалуйста, обновите страницу.');
      } finally {
        setLoading(false);
      }
    };

    loadAllData();
  }, [getToken, navigate]); 

  const handleUserProfileClick = (userId) => {
    navigate(`/profile/${userId}`);
  };

  const getCurrentUsers = () => {
    if (activeTab === 'received') return receivedUsers;
    if (activeTab === 'sent') return sentUsers;
    return favoriteUsers;
  };

  const currentUsers = getCurrentUsers();
  const currentUsersCount = currentUsers.length;
  const receivedCount = receivedUsers.length;
  const sentCount = sentUsers.length;
  const favoritesCount = favoriteUsers.length;

  return (
    <>
      <Header />
      <div className="suggestions-page">
        <div className="suggestions-container">
          <h1 className="page-title">Предложения</h1>
          <div className="suggestions-tabs">
            <button 
              className={`tab ${activeTab === 'received' ? 'active' : ''}`}
              onClick={() => setActiveTab('received')}
            >
              Предложения мне
              {receivedCount > 0 && (
                <span className="tab-badge">{receivedCount}</span>
              )}
            </button>
            
            <button 
              className={`tab ${activeTab === 'sent' ? 'active' : ''}`}
              onClick={() => setActiveTab('sent')}
            >
              Мои предложения
              {sentCount > 0 && (
                <span className="tab-badge">{sentCount}</span>
              )}
            </button>
            
            <button 
              className={`tab ${activeTab === 'favorites' ? 'active' : ''}`}
              onClick={() => setActiveTab('favorites')}
            >
              Избранное
              {favoritesCount > 0 && (
                <span className="tab-badge">{favoritesCount}</span>
              )}
            </button>
          </div>

          {/* Контент табов */}
          <div className="tab-content">
            {loading ? (
              <div className="loading-spinner" style={{ textAlign: 'center', padding: '40px' }}>
                Загрузка всех данных...
              </div>
            ) : error ? (
              <div className="error-message" style={{ color: 'red', padding: '20px', textAlign: 'center' }}>
                {error}
              </div>
            ) : (
              <>
                {tabLoading && (
                  <div style={{ textAlign: 'center', padding: '10px', color: '#666' }}>
                    Обновление данных...
                  </div>
                )}
                
                <div className="suggestions-list">              
                  {currentUsersCount > 0 ? (
                    <div className="cards-grid">
                      {currentUsers.map((user) => (
                        <UserCard
                          key={user.id}
                          user={user}
                          onProfileClick={handleUserProfileClick}
                        />
                      ))}
                    </div>
                  ) : (
                    <div className="empty-state">
                      <div className="empty-icon">
                        {activeTab === 'received' ? '📭' : 
                         activeTab === 'sent' ? '📤' : '⭐'}
                      </div>
                      <h3>
                        {activeTab === 'received' ? 'Пока нет предложений' :
                         activeTab === 'sent' ? 'Вы еще не отправили предложений' :
                         'Избранное пусто'}
                      </h3>
                      <p>
                        {activeTab === 'received' ? 'Обновите свой профиль, чтобы привлечь больше внимания' :
                         activeTab === 'sent' ? 'Найдите интересных музыкантов и предложите им сотрудничество' :
                         'Добавляйте понравившихся музыкантов в избранное'}
                      </p>
                    </div>
                  )}
                </div>
              </>
            )}
          </div>
        </div>
      </div>
    </>
  );
}

export default SuggestionsPage;