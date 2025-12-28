import React, { useState, useRef, useEffect } from 'react';
import './CityFilter.css';

const CityFilter = ({ 
  selectedCity, 
  onCityChange, 
  cities = [],
  placeholder = "Выберите город",
  allCitiesText = "Все города"
}) => {
  const [isOpen, setIsOpen] = useState(false);
  const dropdownRef = useRef(null);

  useEffect(() => {
    const handleClickOutside = (event) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
        setIsOpen(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, []);

  const toggleDropdown = () => setIsOpen(!isOpen);

  const handleCitySelect = (city) => {
    // Если city - это специальный вариант "Все города" или null/undefined
    if (city === allCitiesText || !city) {
      onCityChange('');
    } else {
      // city теперь объект {id: number, name: string, localizedName: string}
      // Передаем ID города
      onCityChange(city.id); // или просто city.id, если хотите число
    }
    setIsOpen(false);
  };

  const clearCity = () => {
    onCityChange('');
    setIsOpen(false);
  };

  // Находим выбранный город для отображения названия
  const getSelectedCityName = () => {
    if (!selectedCity) return placeholder;
    
    // Ищем город по ID
    const foundCity = cities.find(city => 
      city.id.toString() === selectedCity.toString()
    );
    
    return foundCity 
      ? (foundCity.localizedName || foundCity.name)
      : placeholder;
  };

  return (
    <div className="city-filter" ref={dropdownRef}>
      <label className="filter-label">Город:</label>
      <div className="city-filter-container">
        <button
          type="button"
          className="city-filter-button"
          onClick={toggleDropdown}
        >
          <span className="city-filter-placeholder">
            {getSelectedCityName()}
          </span>
          <span className={`city-filter-arrow ${isOpen ? 'open' : ''}`}>
            ▼
          </span>
        </button>
        
        {selectedCity && (
          <button
            type="button"
            className="city-clear-btn"
            onClick={clearCity}
            aria-label="Очистить выбор города"
            title="Очистить выбор города"
          >
            ×
          </button>
        )}
        
        {isOpen && (
          <div className="city-filter-dropdown">
            <div className="city-filter-options">
              {/* Вариант "Все города" */}
              <button
                type="button"
                className={`city-option ${!selectedCity ? 'selected' : ''}`}
                onClick={() => handleCitySelect(null)}
              >
                {allCitiesText}
              </button>
              
              {/* Список городов с объектами */}
              {cities.map((city) => (
                <button
                  key={city.id}
                  type="button"
                  className={`city-option ${
                    selectedCity === city.id.toString() ? 'selected' : ''
                  }`}
                  onClick={() => handleCitySelect(city)} // Передаем объект города
                >
                  {city.localizedName || city.name}
                </button>
              ))}
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default CityFilter;