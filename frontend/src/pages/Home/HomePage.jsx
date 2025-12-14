import React, { useState } from 'react';
import Header from '../../components/Header/Header';
import './HomePage.css';

function HomePage() {
  const [searchQuery, setSearchQuery] = useState('');
  const [activityTypes, setActivityTypes] = useState([]);
  const [genres, setGenres] = useState([]);
  const [experienceFrom, setExperienceFrom] = useState('');
  const [experienceTo, setExperienceTo] = useState('');
  const [searchCommercial, setSearchCommercial] = useState(false);
  const [searchBand, setSearchBand] = useState(false);
  const [searchTeam, setSearchTeam] = useState(false);
  const [filtersOpen, setFiltersOpen] = useState(false);

  const activityOptions = [
    { id: 1, label: 'Вокал' },
    { id: 2, label: 'Гитара' },
    { id: 3, label: 'Бас-гитара' },
    { id: 4, label: 'Ударные' },
  ];

  const genreOptions = [
    { id: 1, label: 'Рок' },
    { id: 2, label: 'Джаз' },
    { id: 3, label: 'Поп' },
    { id: 4, label: 'Хип-хоп' },
  ];

  const handleActivityChange = (id) => {
    setActivityTypes(prev =>
      prev.includes(id) ? prev.filter(item => item !== id) : [...prev, id]
    );
  };

  const handleGenreChange = (id) => {
    setGenres(prev =>
      prev.includes(id) ? prev.filter(item => item !== id) : [...prev, id]
    );
  };

  const handleSearch = (e) => {
    e.preventDefault();
    console.log({
      searchQuery,
      activityTypes,
      genres,
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
              placeholder="Поиск по имени, инструменту, жанру..."
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
                <label>Вид деятельности:</label>
                <div className="checkbox-list">
                  {activityOptions.map(option => (
                    <label key={option.id} className="checkbox-label">
                      <input
                        type="checkbox"
                        checked={activityTypes.includes(option.id)}
                        onChange={() => handleActivityChange(option.id)}
                      />
                      {option.label}
                    </label>
                  ))}
                </div>
              </div>

              <div className="filter-group">
                <label>Жанр:</label>
                <div className="checkbox-list">
                  {genreOptions.map(option => (
                    <label key={option.id} className="checkbox-label">
                      <input
                        type="checkbox"
                        checked={genres.includes(option.id)}
                        onChange={() => handleGenreChange(option.id)}
                      />
                      {option.label}
                    </label>
                  ))}
                </div>
              </div>

              <div className="filter-group">
                <label>Стаж (лет):</label>
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

              <div className="filter-group">
                <label>Статус поиска:</label>
                <div className="status-checkboxes">
                  <label className="checkbox-label">
                    <input
                      type="checkbox"
                      checked={searchCommercial}
                      onChange={(e) => setSearchCommercial(e.target.checked)}
                    />
                    Ищу коммерцию
                  </label>
                  <label className="checkbox-label">
                    <input
                      type="checkbox"
                      checked={searchBand}
                      onChange={(e) => setSearchBand(e.target.checked)}
                    />
                    Ищу коллектив
                  </label>
                  <label className="checkbox-label">
                    <input
                      type="checkbox"
                      checked={searchTeam}
                      onChange={(e) => setSearchTeam(e.target.checked)}
                    />
                    Ищу команду
                  </label>
                </div>
              </div>
            </div>
          </div>
        )}
      </div>
    </>
  );
}

export default HomePage;