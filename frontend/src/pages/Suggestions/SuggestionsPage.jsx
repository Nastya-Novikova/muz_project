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
  
  // Данные для каждой вкладки
  const [receivedData, setReceivedData] = useState([]);
  const [sentData, setSentData] = useState([]);
  const [favoritesData, setFavoritesData] = useState([]);
  const [favoriteIds, setFavoriteIds] = useState(new Set());

    // Универсальная функция для получения массива из любого ответа API
  const extractUsersFromResponse = (response, tabName) => {
    if (!response) return [];
    
    console.log(`Ответ для ${tabName}:`, response);
    
    // Проверяем разные возможные структуры ответа
    if (tabName === 'favorites' && response.favorites && Array.isArray(response.favorites)) {
      return response.favorites;
    }
    
    if ((tabName === 'received' || tabName === 'sent') && 
        response.suggestions && Array.isArray(response.suggestions)) {
      return response.suggestions;
    }
    
    return [];
  };

   useEffect(() => {
    const loadData = async () => {
      setLoading(true);
      setError('');
      
      try {
        const token = getToken();
        if (!token) {
          navigate('/login');
          return;
        }

        let response;
        let users = [];
        
        if (activeTab === 'received') {
          response = await api.getReceivedSuggestions(token);
          users = extractUsersFromResponse(response, 'received');
          setReceivedData(users);
        } else if (activeTab === 'sent') {
          response = await api.getSentSuggestions(token);
          users = extractUsersFromResponse(response, 'sent');
          setSentData(users);
        } else if (activeTab === 'favorites') {
          response = await api.getFavorites(token);
          users = extractUsersFromResponse(response, 'favorites');
          setFavoritesData(users);
          
          // Сохраняем ID избранных пользователей
          const ids = new Set(users.map(user => user.id));
          setFavoriteIds(ids);
        }
        
        console.log(`Загружены данные для вкладки ${activeTab}:`, response);
        
      } catch (err) {
        console.error('Ошибка загрузки данных:', err);
        setError('Не удалось загрузить данные');
      } finally {
        setLoading(false);
      }
    };

    loadData();
  }, [activeTab, getToken, navigate]);

  const handleUserProfileClick = (userId) => {
    navigate(`/profile/${userId}`);
  };

  // Получаем данные для текущей вкладки
  const getCurrentData = () => {
    if (activeTab === 'received') return receivedData;
    if (activeTab === 'sent') return sentData;
    return favoritesData;
  };

  const users = getCurrentData();

  return (
    <>
      <Header />
      <div className="suggestions-page">
        <div className="suggestions-container">
          <h1 className="page-title">Предложения</h1>
          
          {/* Табы */}
          <div className="suggestions-tabs">
            <button 
              className={`tab ${activeTab === 'received' ? 'active' : ''}`}
              onClick={() => setActiveTab('received')}
            >
              Предложения мне
              {users.length > 0 && activeTab === 'received' && (
                <span className="tab-badge">{users.length}</span>
              )}
            </button>
            
            <button 
              className={`tab ${activeTab === 'sent' ? 'active' : ''}`}
              onClick={() => setActiveTab('sent')}
            >
              Мои предложения
            </button>
            
            <button 
              className={`tab ${activeTab === 'favorites' ? 'active' : ''}`}
              onClick={() => setActiveTab('favorites')}
            >
              Избранное
              {users.length > 0 && activeTab === 'favorites' && (
                <span className="tab-badge">{users.length}</span>
              )}
            </button>
          </div>

          {/* Контент табов */}
          <div className="tab-content">
            {error && (
              <div className="error-message" style={{ color: 'red', padding: '20px', textAlign: 'center' }}>
                {error}
              </div>
            )}

            {loading ? (
              <div className="loading-spinner" style={{ textAlign: 'center', padding: '40px' }}>
                Загрузка...
              </div>
            ) : (
              <div className="suggestions-list">              
                {users.length > 0 ? (
                  <div className="cards-grid">
                    {users.map((user) => (
                        <UserCard
                          key={user.id}
                          user={user}
                          onProfileClick={handleUserProfileClick}
                        />
                      ))
                    }  
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
            )}
          </div>
        </div>
      </div>
    </>
  );
}

export default SuggestionsPage;