import React, { useEffect } from 'react';
import './ConfirmDeleteModal.css';

function ConfirmDeleteModal({ isOpen, onClose, onConfirm, userName }) {
  
  useEffect(() => {
    if (isOpen) {
      document.body.classList.add('modal-open');
    } else {
      document.body.classList.remove('modal-open');
    }
    return () => {
      document.body.classList.remove('modal-open');
    };
  }, [isOpen]);

  if (!isOpen) return null;

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <h2 className="modal-title">Удаление профиля</h2>
        <p className="modal-text">
          Вы уверены, что хотите удалить профиль <strong>{userName}</strong>?
          Это действие необратимо.
        </p>
        <div className="modal-actions">
          <button onClick={onClose} className="modal-btn modal-btn-cancel">
            Отмена
          </button>
          <button onClick={onConfirm} className="modal-btn modal-btn-delete">
            Удалить
          </button>
        </div>
      </div>
    </div>
  );
}

export default ConfirmDeleteModal;