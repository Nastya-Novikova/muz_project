import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom'; 
import Header from '../../components/Header/Header';
import MultiSelectDropdown from '../../components/MultiSelectDropDown/MultiSelectDropDown';
import CityFilter from '../../components/CityFilter/CityFilter';
import UserCard from '../../components/UserCard/UserCard';
import { useFilters } from '../../context/useFilters';
import { api } from '../../services/api';
import './HomePage.css';

function HomePage() {
  const [searchQuery, setSearchQuery] = useState('');
  const [activityTypes, setActivityTypes] = useState([]);
  const [genres, setGenres] = useState([]);
  const [city, setCity] = useState(''); 
  const [experienceMin, setExperienceMin] = useState('');
  const [experienceMax, setExperienceMax] = useState('');
  const [filtersOpen, setFiltersOpen] = useState(false);

  const [searchResults, setSearchResults] = useState([]);
  const [loading, setLoading] = useState(false);
  const [searchError, setSearchError] = useState('');

  const { cities, activities, genres: genreData } = useFilters();
  const navigate = useNavigate();

  const handleSearch = async (e) => {
    e.preventDefault();
    setLoading(true);
    setSearchError('');

    try {
      // 1. Формируем объект параметров для запроса
    // 1. Формируем объект параметров для запроса
    const searchParams = {};
    
    // Добавляем только те параметры, которые есть
    if (searchQuery.trim()) {
      searchParams.query = searchQuery.trim();
    }
    
    if (activityTypes.length > 0) {
      searchParams.specialtyIds = activityTypes;
    }
    
    if (genres.length > 0) {
      searchParams.genreIds = genres;
    }
    
    if (city && city !== '') {
      const cityId = parseInt(city, 10);
      if (!isNaN(cityId)) {
        searchParams.cityId = cityId;
      }
    }
    
    if (experienceMin) {
      searchParams.experienceMin = parseInt(experienceMin, 10);
    }
    
    if (experienceMax) {
      searchParams.experienceMax = parseInt(experienceMax, 10);
    }

      // 2. Отправляем запрос на сервер
      const response = await api.searchMusicians(Object.keys(searchParams).length > 0 ? searchParams : {}, );

      // 3. Сохраняем результаты
      console.log('Получены результаты поиска:', response);
      const users = response.results || [];
      setSearchResults(users);

    } catch (err) {
      console.error('Ошибка при поиске:', err);
      setSearchError('Не удалось выполнить поиск. Пожалуйста, попробуйте позже.');
      setSearchResults([]); // Очищаем результаты при ошибке
    } finally {
      setLoading(false);
    }
  };

  const handleUserProfileClick = (userId) => {
    navigate(`/profile/${userId}`);
  };

  return (
    <>
      <Header />
      <div className="home-page">
        <form onSubmit={handleSearch} className="search-form">
          <div className="search-input-group">
            <input
              type="text"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              placeholder="Поиск по имени исполнителя или названию коллектива"
              className="search-input"
            />
            <button type="submit" className="search-button" disabled={loading}>Найти</button>
            <button
              type="button"
              onClick={() => setFiltersOpen(!filtersOpen)}
              className="toggle-filters-button"
            >
              {filtersOpen ? 'Скрыть фильтры' : 'Показать фильтры'}
            </button>
          </div>
        </form>

        {searchError && <div className="search-error-message">{searchError}</div>}

        {filtersOpen && (
          <div className="filters-panel">
            <div className="filters-panel-grid">
              <div className="filter-group">
                <MultiSelectDropdown
                  label="Вид деятельности"
                  options={activities}
                  selectedIds={activityTypes}
                  onChange={setActivityTypes}
                  placeholder="Выберите виды деятельности..."
                  allText="Все виды"
                />
              </div>

              <div className="filter-group">
                <MultiSelectDropdown
                  label="Жанр"
                  options={genreData}
                  selectedIds={genres}
                  onChange={setGenres}
                  placeholder="Выберите жанры..."
                  allText="Все жанры"
                />
              </div>

              <div className="filter-group">
                <CityFilter
                  selectedCity={city}
                  onCityChange={setCity}
                  cities = {cities}
                  placeholder="Выберите город"
                  allCitiesText="Все города"
                />
              </div>

              <div className="filter-group">
                <label className="filter-group-label">Стаж (лет):</label>
                <div className="experience-inputs">
                  <input
                    type="number"
                    value={experienceMin}
                    onChange={(e) => setExperienceMin(e.target.value)}
                    placeholder="От"
                    className="experience-input"
                    min="0"
                  />
                  <input
                    type="number"
                    value={experienceMax}
                    onChange={(e) => setExperienceMax(e.target.value)}
                    placeholder="До"
                    className="experience-input"
                    min="0"
                  />
                </div>
              </div>
            </div>
          </div>
        )}

        <div className="cards-preview">
          <h2 className="preview-title">Результаты поиска</h2>         
          {loading ? (
            <div className="loading-spinner">Загрузка...</div>
          ) : (
            <div className="cards-grid">
              {/* 2. Размещаем карточки пользователей из ответа */}
              {searchResults.length > 0 ? (
                searchResults.map((user) => (
                  <UserCard
                    key={user.id}
                    user={user}
                    // 3. Передаем функцию для обработки клика по карточке
                    onProfileClick={handleUserProfileClick}
                  />
                ))
              ) : (
                <p className="no-results">
                  {searchError ? 'Ошибка при поиске' : 'По вашему запросу ничего не найдено. Попробуйте изменить параметры.'}
                </p>
              )}
            </div>
          )}
        </div>
      </div>
    </>
  );
}

export default HomePage;