import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Header from '../../components/Header/Header';
import UserCard from '../../components/UserCard/UserCard';
import './SuggestionsPage.css';

function SuggestionsPage() {
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState('received');
  const [favorites, setFavorites] = useState(['1', '3']); 

const mockUsers = [
  {
    id: '1',
    fullName: 'Петров Алексей Сергеевич',
    age: 32,
    avatar: 'https://ui-avatars.com/api/?name=Алексей+Петров&background=667eea',
    activityType: 'Ударные',
    genres: ['Рок', 'Метал', 'Хард-рок'],
    experience: 8,
    description: 'Профессиональный барабанщик с опытом выступлений на крупных площадках.',
    city: 'Санкт-Петербург'
  },
  {
    id: '2',
    fullName: 'Смирнова Анна Дмитриевна',
    age: 26,
    avatar: 'https://ui-avatars.com/api/?name=Анна+Смирнова&background=f56565',
    activityType: 'Вокал',
    genres: ['Поп', 'Джаз', 'Соул'],
    experience: 4,
    description: 'Джазовая вокалистка, выпускница музыкального колледжа.',
    city: 'Москва'
  },
  {
    id: '3',
    fullName: 'Козлов Денис Игоревич',
    age: 35,
    avatar: 'https://ui-avatars.com/api/?name=Денис+Козлов&background=48bb78',
    activityType: 'Гитара, Композитор',
    genres: ['Блюз', 'Рок', 'Фолк'],
    experience: 12,
    description: 'Гитарист-композитор. Пишу музыку в стиле блюз-рок.',
    city: 'Екатеринбург'
  },
  {
    id: '4',
    fullName: 'Николаева Мария Павловна',
    age: 29,
    avatar: 'https://ui-avatars.com/api/?name=Мария+Николаева&background=ed8936',
    activityType: 'Клавишные',
    genres: ['Электроника', 'Эмбиент', 'Синт-поп'],
    experience: 6,
    description: 'Электронный музыкант, работаю с синтезаторами.',
    city: 'Новосибирск'
  },
  {
    id: '5',
    fullName: 'Волков Игорь Александрович',
    age: 40,
    avatar: 'https://ui-avatars.com/api/?name=Игорь+Волков&background=4299e1',
    activityType: 'Бас-гитара',
    genres: ['Фанк', 'Диско', 'Соул'],
    experience: 15,
    description: 'Опытный бас-гитарист, специализируюсь на фанке и диско.',
    city: 'Краснодар'
  },
  {
    id: '6',
    fullName: 'Федорова Екатерина Викторовна',
    age: 24,
    avatar: 'https://ui-avatars.com/api/?name=Екатерина+Федорова&background=9f7aea',
    activityType: 'Скрипка',
    genres: ['Классика', 'Неоклассика', 'Пост-рок'],
    experience: 3,
    description: 'Скрипачка, играю как классическую, так и современную музыку.',
    city: 'Казань'
  }
];

  const mockSuggestions = {
    received: mockUsers.slice(0, 3), 
    sent: mockUsers.slice(3, 5), 
    favorites: mockUsers.filter(user => favorites.includes(user.id)) 
  };

  const handleFavoriteClick = (userId) => {
    setFavorites(prev => 
      prev.includes(userId) 
        ? prev.filter(id => id !== userId)
        : [...prev, userId]
    );
  };

  const handleProfileClick = (userId) => {
    navigate(`/profile/${userId}`);
  };

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
              {mockSuggestions.received.length > 0 && (
                <span className="tab-badge">{mockSuggestions.received.length}</span>
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
              <span className="tab-badge">{mockSuggestions.favorites.length}</span>
            </button>
          </div>

          {/* Контент табов */}
          <div className="tab-content">
            {activeTab === 'received' && (
              <div className="suggestions-list">              
                {mockSuggestions.received.length > 0 ? (
                  <div className="cards-grid">
                    {mockSuggestions.received.map(user => (
                      <UserCard
                        key={user.id}
                        user={user}
                        isFavorite={favorites.includes(user.id)}
                        onFavoriteClick={handleFavoriteClick}
                      />
                    ))}
                  </div>
                ) : (
                  <div className="empty-state">
                    <div className="empty-icon">📭</div>
                    <h3>Пока нет предложений</h3>
                    <p>Обновите свой профиль, чтобы привлечь больше внимания</p>
                  </div>
                )}
              </div>
            )}

            {activeTab === 'sent' && (
              <div className="suggestions-list">               
                {mockSuggestions.sent.length > 0 ? (
                  <div className="cards-grid">
                    {mockSuggestions.sent.map(user => (
                      <UserCard
                        key={user.id}
                        user={user}
                        isFavorite={favorites.includes(user.id)}
                        onFavoriteClick={handleFavoriteClick}
                      />
                    ))}
                  </div>
                ) : (
                  <div className="empty-state">
                    <div className="empty-icon">📤</div>
                    <h3>Вы еще не отправили предложений</h3>
                    <p>Найдите интересных музыкантов и предложите им сотрудничество</p>
                  </div>
                )}
              </div>
            )}

            {activeTab === 'favorites' && (
              <div className="suggestions-list">               
                {mockSuggestions.favorites.length > 0 ? (
                  <div className="cards-grid">
                    {mockSuggestions.favorites.map(user => (
                      <UserCard
                        key={user.id}
                        user={user}
                        isFavorite={favorites.includes(user.id)}
                        onFavoriteClick={handleFavoriteClick}
                      />
                    ))}
                  </div>
                ) : (
                  <div className="empty-state">
                    <div className="empty-icon">⭐</div>
                    <h3>Избранное пусто</h3>
                    <p>Добавляйте понравившихся музыкантов в избранное</p>
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