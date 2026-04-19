// frontend/src/pages/Events/EventsPage.jsx
import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../../context/AuthContext';
import { useFilters } from '../../../context/useFilters';
import { api } from '../../../services/api';
import Header from '../../../components/Header/Header';
import EventCard from '../../../components/EventCard/EventCard';
import SelectFilter from '../../../components/SelectFilter/SelectFilter';
import './EventsPage.css';

function EventsPage() {
  const navigate = useNavigate();
  const { getToken } = useAuth();
  const { cities, regions } = useFilters();
  
  const [activeTab, setActiveTab] = useState('all');
  const [loading, setLoading] = useState(true);
  const [events, setEvents] = useState([]);
  const [myCreatedEvents, setMyCreatedEvents] = useState([]);
  const [myRegisteredEvents, setMyRegisteredEvents] = useState([]);
  const [error, setError] = useState('');

  // Состояние для фильтров
  const [searchQuery, setSearchQuery] = useState('');
  const [filtersOpen, setFiltersOpen] = useState(false);
  const [filters, setFilters] = useState({
    regionId: '',
    cityId: '',
    fromDate: '',
    toDate: '',
  });

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

        const [allEvents, createdEvents, registeredEvents] = await Promise.all([
          api.getEvents({ page: 1, limit: 100 }),
          api.getMyCreatedEvents(token),
          api.getMyRegisteredEvents(token),
        ]);

        setEvents(allEvents.items || []);
        setMyCreatedEvents(createdEvents.items || []);
        setMyRegisteredEvents(registeredEvents.items || []);
      } catch (err) {
        console.error('Ошибка загрузки мероприятий:', err);
        setError('Не удалось загрузить мероприятия.');
      } finally {
        setLoading(false);
      }
    };

    loadData();
  }, [getToken, navigate]);

  const handleFilterChange = (e) => {
    const { name, value } = e.target;
    setFilters(prev => ({ ...prev, [name]: value }));
  };

  const handleRegionChange = (regionId) => {
    setFilters(prev => ({ ...prev, regionId: regionId || '' }));
  };

  const handleCityChange = (cityId) => {
    setFilters(prev => ({ ...prev, cityId: cityId || '' }));
  };

  const handleSearch = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      const filterParams = {
        query: searchQuery || undefined,
        regionId: filters.regionId || undefined,
        cityId: filters.cityId || undefined,
        fromDate: filters.fromDate || undefined,
        toDate: filters.toDate || undefined,
        page: 1,
        limit: 100,
      };
      Object.keys(filterParams).forEach(key => {
        if (filterParams[key] === undefined) delete filterParams[key];
      });

      const response = await api.getEvents(filterParams);
      setEvents(response.items || []);
    } catch (err) {
      console.error('Ошибка фильтрации:', err);
      setError('Не удалось применить фильтры.');
    } finally {
      setLoading(false);
    }
  };

  const handleCreateEvent = () => {
    navigate('/events/create');
  };

  const getCurrentEvents = () => {
    if (activeTab === 'all') return events;
    if (activeTab === 'created') return myCreatedEvents;
    return myRegisteredEvents;
  };

  const currentEvents = getCurrentEvents();

  return (
    <>
      <Header />
      <div className="events-page">
        <div className="events-container">
          <div className="events-tabs">
            <button className={`tab ${activeTab === 'all' ? 'active' : ''}`} onClick={() => setActiveTab('all')}>
              Все мероприятия
            </button>
            <button className={`tab ${activeTab === 'created' ? 'active' : ''}`} onClick={() => setActiveTab('created')}>
              Мои мероприятия
            </button>
            <button className={`tab ${activeTab === 'registered' ? 'active' : ''}`} onClick={() => setActiveTab('registered')}>
              Я участвую
            </button>
          </div>

          <div className="tab-content">
            {activeTab === 'all' && (
              <>
                <form onSubmit={handleSearch} className="search-form">
                  <div className="search-input-group">
                    <input
                      type="text"
                      value={searchQuery}
                      onChange={(e) => setSearchQuery(e.target.value)}
                      placeholder="Поиск по названию или описанию"
                      className="search-input"
                    />
                    <button type="submit" className="search-button" disabled={loading}>
                      Найти
                    </button>
                    <button
                      type="button"
                      onClick={() => setFiltersOpen(!filtersOpen)}
                      className="toggle-filters-button"
                    >
                      {filtersOpen ? 'Скрыть фильтры' : 'Показать фильтры'}
                    </button>
                  </div>

                  {filtersOpen && (
                    <div className="filters-panel">
                      <div className="filters-panel-grid">
                        <div className="filter-group">
                          <SelectFilter
                            label="Регион"
                            selectedValue={filters.regionId}
                            onChange={handleRegionChange}
                            options={regions}
                            placeholder="Выберите регион"
                            allOptionText="Все регионы"
                          />
                        </div>

                        <div className="filter-group">
                          <SelectFilter
                            label="Город"
                            selectedValue={filters.cityId}
                            onChange={handleCityChange}
                            options={cities}
                            placeholder="Выберите город"
                            allOptionText="Все города"
                          />
                        </div>

                        <div className="filter-group">
                          <label className="filter-group-label">Дата с:</label>
                          <input
                            type="date"
                            name="fromDate"
                            value={filters.fromDate}
                            onChange={handleFilterChange}
                            className="filter-input"
                          />
                        </div>

                        <div className="filter-group">
                          <label className="filter-group-label">Дата по:</label>
                          <input
                            type="date"
                            name="toDate"
                            value={filters.toDate}
                            onChange={handleFilterChange}
                            className="filter-input"
                          />
                        </div>
                      </div>
                    </div>
                  )}
                </form>
              </>
            )}

            {activeTab === 'created' && (
              <div className="create-event-btn-container">
                <button className="create-event-btn" onClick={handleCreateEvent}>
                  + Создать мероприятие
                </button>
              </div>
            )}

            {loading ? (
              <div className="loading-spinner">Загрузка...</div>
            ) : error ? (
              <div className="error-message">{error}</div>
            ) : currentEvents.length > 0 ? (
              <div className="events-grid">
                {currentEvents.map(event => (
                  <EventCard key={event.id} event={event} />
                ))}
              </div>
            ) : (
              <div className="empty-state">
                <h3>Мероприятий не найдено</h3>
                <p>{activeTab === 'all' ? 'Попробуйте изменить параметры поиска.' : 'Здесь будут отображаться ваши мероприятия.'}</p>
              </div>
            )}
          </div>
        </div>
      </div>
    </>
  );
}

export default EventsPage;