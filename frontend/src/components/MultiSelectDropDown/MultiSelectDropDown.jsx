import React, { useState, useRef, useEffect } from 'react';
import './MultiSelectDropDown.css';

const MultiSelectDropdown = ({ 
  label, 
  options, 
  selectedIds, 
  onChange, 
  placeholder = 'Выберите...',
  allText = 'Все'
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

  const toggleDropdown = () => {
    setIsOpen(!isOpen);
  };

  const handleOptionChange = (id) => {
    const newSelected = selectedIds.includes(id)
      ? selectedIds.filter(item => item !== id)
      : [...selectedIds, id];
    onChange(newSelected);
  };

  const selectAll = () => {
    onChange(options.map(option => option.id));
  };

  const clearAll = () => {
    onChange([]);
  };

  const getDisplayText = () => {
    if (selectedIds.length === 0) return placeholder;
    if (selectedIds.length === options.length) return allText;
    
    const selectedCount = selectedIds.length;
    const firstSelectedOption = options.find(opt => selectedIds.includes(opt.id));
    
    if (selectedCount === 1) return firstSelectedOption.label;
    return `${firstSelectedOption.label} + ещё ${selectedCount - 1}`;
  };

  return (
    <div className="multi-select-dropdown" ref={dropdownRef}>
      {label && <label className="dropdown-label">{label}:</label>}
      <div className="dropdown-container">
        <button
          type="button"
          className="dropdown-button"
          onClick={toggleDropdown}
        >
          <span className="dropdown-display-text">
            {getDisplayText()}
          </span>
          <span className={`dropdown-arrow ${isOpen ? 'open' : ''}`}>
            ▼
          </span>
        </button>
        
        {isOpen && (
          <div className="dropdown-menu">
            <div className="dropdown-header">
              <button 
                type="button" 
                className="dropdown-select-all"
                onClick={selectAll}
              >
                Выбрать все
              </button>
              <button 
                type="button" 
                className="dropdown-clear-all"
                onClick={clearAll}
              >
                Очистить все
              </button>
            </div>
            <div className="dropdown-options">
              {options.map(option => (
                <label key={option.id} className="dropdown-option">
                  <input
                    type="checkbox"
                    checked={selectedIds.includes(option.id)}
                    onChange={() => handleOptionChange(option.id)}
                    className="dropdown-checkbox"
                  />
                  <span className="dropdown-option-label">{option.label}</span>
                </label>
              ))}
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default MultiSelectDropdown;