// frontend/src/components/SelectFilter/SelectFilter.jsx
import React, { useState, useRef, useEffect } from 'react';
import './SelectFilter.css';

const SelectFilter = ({ 
  label,
  selectedValue, 
  onChange, 
  options = [],
  placeholder = "Выберите...",
  allOptionText = "Все"
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

  const handleSelect = (value) => {
    onChange(value);
    setIsOpen(false);
  };

  const clearSelection = () => {
    onChange('');
    setIsOpen(false);
  };

  const getSelectedOptionName = () => {
    if (!selectedValue) return placeholder;
    
    const foundOption = options.find(option => 
      option.id.toString() === selectedValue.toString()
    );
    
    return foundOption 
      ? (foundOption.localizedName || foundOption.name)
      : placeholder;
  };

  return (
    <div className="select-filter" ref={dropdownRef}>
      {label && <label className="filter-label">{label}:</label>}
      <div className="select-filter-container">
        <button
          type="button"
          className="select-filter-button"
          onClick={toggleDropdown}
        >
          <span className="select-filter-placeholder">
            {getSelectedOptionName()}
          </span>
          <span className={`select-filter-arrow ${isOpen ? 'open' : ''}`}>
            ▼
          </span>
        </button>
        
        {selectedValue && (
          <button
            type="button"
            className="select-clear-btn"
            onClick={clearSelection}
            aria-label="Очистить выбор"
            title="Очистить выбор"
          >
            ×
          </button>
        )}
        
        {isOpen && (
          <div className="select-filter-dropdown">
            <div className="select-filter-options">
              <button
                type="button"
                className={`select-option ${!selectedValue ? 'selected' : ''}`}
                onClick={() => handleSelect('')}
              >
                {allOptionText}
              </button>
              
              {options.map((option) => (
                <button
                  key={option.id}
                  type="button"
                  className={`select-option ${
                    selectedValue === option.id.toString() ? 'selected' : ''
                  }`}
                  onClick={() => handleSelect(option.id.toString())}
                >
                  {option.localizedName || option.name}
                </button>
              ))}
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default SelectFilter;