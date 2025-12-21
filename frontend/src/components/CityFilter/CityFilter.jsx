import React, { useState, useRef, useEffect } from 'react';
import './CityFilter.css';

const CityFilter = ({ 
  selectedCity, 
  onCityChange, 
  placeholder = "Выберите город",
  allCitiesText = "Все города"
}) => {
  const [isOpen, setIsOpen] = useState(false);
  const dropdownRef = useRef(null);

  const cityOptions = [
    'Москва',
    'Санкт-Петербург',
    'Новосибирск',
    'Екатеринбург',
    'Казань',
    'Нижний Новгород',
  ];

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

  const toggleDropdown = () => {
    setIsOpen(!isOpen);
  };

  const handleCitySelect = (city) => {
    onCityChange(city === allCitiesText ? '' : city);
    setIsOpen(false);
  };

  const clearCity = () => {
    onCityChange('');
    setIsOpen(false);
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
            {selectedCity || placeholder}
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
              <button
                type="button"
                className={`city-option ${!selectedCity ? 'selected' : ''}`}
                onClick={() => handleCitySelect(allCitiesText)}
              >
                {allCitiesText}
              </button>
              
              {cityOptions.map(city => (
                <button
                  key={city}
                  type="button"
                  className={`city-option ${selectedCity === city ? 'selected' : ''}`}
                  onClick={() => handleCitySelect(city)}
                >
                  {city}
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