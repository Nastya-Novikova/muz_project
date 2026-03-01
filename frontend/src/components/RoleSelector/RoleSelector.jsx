import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './RoleSelector.css';

function RoleSelector({ onRoleSelect }) {
  const [selectedRole, setSelectedRole] = useState('');
  const navigate = useNavigate();

  const handleSubmit = () => {
    if (!selectedRole) {
      alert('Пожалуйста, выберите роль');
      return;
    }
    // Заглушка
    localStorage.setItem('userRole', selectedRole);
    onRoleSelect(selectedRole);
    navigate('/profile/edit');
  };

  return (
    <div className="role-selector-overlay">
      <div className="role-selector-container">
        <h2>Кем вы являетесь?</h2>
        <p>Выберите один вариант</p>
        
        <div className="role-options">
          <label className={`role-option ${selectedRole === 'musician' ? 'selected' : ''}`}>
            <input
              type="radio"
              name="role"
              value="musician"
              checked={selectedRole === 'musician'}
              onChange={(e) => setSelectedRole(e.target.value)}
            />
            <span className="role-label">🎸 Музыкант</span>
          </label>
          
          <label className={`role-option ${selectedRole === 'band' ? 'selected' : ''}`}>
            <input
              type="radio"
              name="role"
              value="band"
              checked={selectedRole === 'band'}
              onChange={(e) => setSelectedRole(e.target.value)}
            />
            <span className="role-label">🎤 Коллектив</span>
          </label>
        </div>

        <button 
          onClick={handleSubmit}
          className="role-submit-btn"
          disabled={!selectedRole}
        >
          Продолжить
        </button>
      </div>
    </div>
  );
}

export default RoleSelector;