import React, { useState } from 'react';
import Header from '../../components/Header/Header';
import MultiSelectDropdown from '../../components/MultiSelectDropDown/MultiSelectDropDown';
import CityFilter from '../../components/CityFilter/CityFilter';
import UserCard from '../../components/UserCard/UserCard';
import { useFilters } from '../../context/useFilters';
import './HomePage.css';

function HomePage() {
  const [searchQuery, setSearchQuery] = useState('');
  const [activityTypes, setActivityTypes] = useState([]);
  const [genres, setGenres] = useState([]);
  const [city, setCity] = useState(''); 
  const [experienceFrom, setExperienceFrom] = useState('');
  const [experienceTo, setExperienceTo] = useState('');
  const [filtersOpen, setFiltersOpen] = useState(false);

  const { cities, activities, genres: genreData } = useFilters();

  const mockUsers = [
    {
      id: '1',
      fullName: 'Иванов Иван Иванович',
      age: 28,
      city: 'Москва',
      avatar: 'https://ui-avatars.com/api/?name=Иван+Иванов&background=random',
      activityType: 'Гитара, Вокал',
      genres: ['Рок', 'Джаз', 'Блюз'],
      experience: 5,
      description: 'Опытный музыкант с 5-летним стажем. Ищу коллектив для создания рок-группы.',
    },
    {
      id: '2',
      fullName: 'Петрова Анна Сергеевна',
      age: 25,
      city: 'Санкт-Петербург',
      avatar: 'https://ui-avatars.com/api/?name=Анна+Петрова&background=667eea',
      activityType: 'Вокал',
      genres: ['Поп', 'Джаз', 'Соул'],
      experience: 4,
      description: 'Джазовая вокалистка с классическим образованием.',
    },
    {
      id: '3',
      fullName: 'Сидоров Алексей Викторович',
      age: 32,
      city: 'Екатеринбург',
      avatar: 'https://ui-avatars.com/api/?name=Алексей+Сидоров&background=48bb78',
      activityType: 'Ударные',
      genres: ['Рок', 'Метал', 'Альтернатива'],
      experience: 8,
      description: 'Профессиональный барабанщик, ищу серьезный музыкальный проект.',
    },
        {
      id: '4',
      fullName: 'Сидоров Алексей Викторович',
      age: 32,
      city: 'Екатеринбург',
      avatar: 'https://ui-avatars.com/api/?name=Алексей+Сидоров&background=48bb78',
      activityType: 'Ударные',
      genres: ['Рок', 'Метал', 'Альтернатива'],
      experience: 8,
      description: 'Профессиональный барабанщик, ищу серьезный музыкальный проект.',
    },
        {
      id: '5',
      fullName: 'Сидоров Алексей Викторович',
      age: 32,
      city: 'Екатеринбург',
      avatar: 'https://ui-avatars.com/api/?name=Алексей+Сидоров&background=48bb78',
      activityType: 'Ударные',
      genres: ['Рок', 'Метал', 'Альтернатива'],
      experience: 8,
      description: 'Профессиональный барабанщик, ищу серьезный музыкальный проект.',
    }
  ];

  const handleSearch = (e) => {
    e.preventDefault();
    console.log({
      searchQuery,
      activityTypes,
      genres,
      city,
      experienceFrom: experienceFrom ? parseInt(experienceFrom, 10) : null,
      experienceTo: experienceTo ? parseInt(experienceTo, 10) : null,
      searchCommercial,
      searchBand,
      searchTeam,
    });
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
            <button type="submit" className="search-button">Найти</button>
            <button
              type="button"
              onClick={() => setFiltersOpen(!filtersOpen)}
              className="toggle-filters-button"
            >
              {filtersOpen ? 'Скрыть фильтры' : 'Показать фильтры'}
            </button>
          </div>
        </form>

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
                    value={experienceFrom}
                    onChange={(e) => setExperienceFrom(e.target.value)}
                    placeholder="От"
                    className="experience-input"
                    min="0"
                  />
                  <input
                    type="number"
                    value={experienceTo}
                    onChange={(e) => setExperienceTo(e.target.value)}
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
          <div className="cards-grid">
            {mockUsers.map(user => (
              <UserCard
                key={user.id}
                user={user}
                showActions={true}
              />
            ))}
          </div>
        </div>
      </div>
    </>
  );
}

export default HomePage;