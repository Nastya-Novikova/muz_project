import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import './RoleSelector.css';

function RoleSelector({ onRoleSelect }) {
  const [selectedRole, setSelectedRole] = useState('');
  const { setRole } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = () => {
    if (!selectedRole) {
      alert('Пожалуйста, выберите роль');
      return;
    }
    setRole(selectedRole);
    onRoleSelect(selectedRole);
    navigate('/profile/edit');
  };

  return (
    <div className="role-selector-overlay">
      <div className="role-selector-container">
        <h2>Я регистрируюсь в качестве:</h2>
        <p>Выберите один вариант</p>
        
        <div className="role-options">
          <label className={`role-option ${selectedRole === 'Individual' ? 'selected' : ''}`}>
            <input
              type="radio"
              name="role"
              value="Individual"
              checked={selectedRole === 'Individual'}
              onChange={(e) => setSelectedRole(e.target.value)}
            />
            <span className="role-label">Музыканта</span>
          </label>
          
          <label className={`role-option ${selectedRole === 'Band' ? 'selected' : ''}`}>
            <input
              type="radio"
              name="role"
              value="Band"
              checked={selectedRole === 'Band'}
              onChange={(e) => setSelectedRole(e.target.value)}
            />
            <span className="role-label">Коллектива</span>
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